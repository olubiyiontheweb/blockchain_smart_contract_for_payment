using System;
using System.Collections.Generic;

namespace SmartCreditorBlockchain
{
    // block producer for the credit _blockChain
    public class CreditBlockProducer
    {
        private List<GroupCreditor> _groupCreditors = new List<GroupCreditor>();
        private List<Block> _blockChain = new List<Block>();
        private List<Credit> _unprocessedCredits = new List<Credit>();

        private int _difficulty = 3;

        public CreditBlockProducer()
        {       
            // creating the first block
            string previousHash = "0";
            Credit credit = new Credit("mined", "lender", 500);
            Block block = new Block(previousHash, credit);            
            block.index = _blockChain.Count;
            block.MineBlock(this._difficulty);
            _blockChain.Add(block);
        }

        public Block CreateBlock(Credit credit, string previousHash)
        {
            Block block = new Block(previousHash, credit);
            block.index = _blockChain.Count;
            block.MineBlock(this._difficulty);
            return block;
        }

        public bool IsValidNewBlock(Block newBlock, Block previousBlock)
        {
            if (previousBlock.hash != newBlock.previousHash)
            {
                Console.WriteLine("\nPrevious hash does not match!");
                return false;
            }
            
            if (newBlock.hash != newBlock.CalculateHash())
            {
                return false;
            }

            // previoud block must be older than the new block
            if (previousBlock.timeStamp.Date > newBlock.timeStamp.Date)
            {
                return false;
            }
            
            return true;
        }

        // add a new block to the _blockChain
        public void AddBlock(Block newBlock)
        {
            Block previousBlock = this._blockChain[this._blockChain.Count - 1];
            if (IsValidNewBlock(newBlock, previousBlock))
            {
                this._blockChain.Add(newBlock);
                Console.WriteLine("Block added to the chain");
            }
        }

        // is the chain valid?
        public bool IsValidChain()
        {
            for (int i = 1; i < this._blockChain.Count; i++)
            {
                Block block = this._blockChain[i];
                Block previousBlock = this._blockChain[i - 1];
                if (block.hash != block.CalculateHash())
                {
                    return false;
                }
                if (block.previousHash != previousBlock.hash)
                {
                    return false;
                }
            }
            return true;
        }

        // get the balance of the creditor or debtor
        public string GetBalance(string address)
        {
            float balance = 0;
            foreach (Block block in this._blockChain)
            {
                // money leaves the creditor
                if (block.credit.creditor.ToLowerInvariant() == address.ToLowerInvariant())
                {
                    balance -= block.credit.amount;
                }

                // money goes to the debtor
                if (block.credit.debtor.ToLowerInvariant() == address.ToLowerInvariant())
                {
                    balance += block.credit.amount;
                }
            }
            return balance.ToString();
        }

        public bool IsValidNewGroup(GroupCreditor group)
        {
            if (group.groupName == "")
            {
                return false;
            }

            if (group.totalAmount <= 0)
            {
                return false;
            }

            if (this._groupCreditors.Contains(group))
            {
                return false;
            }

            return true;
        }

        // create new credit group
        public bool CreateNewGroup(string groupName, float amount, float agreedProportion)
        {
            GroupCreditor group = new GroupCreditor(groupName, amount, agreedProportion);
            group.hash = group.CalculateHash();

            if (IsValidNewGroup(group))
            {
                this._groupCreditors.Add(group);
                return true;
            }

            return false;
        }

        // is the new credit transaction valid?
        public bool IsValidNewCredit(Credit credit)
        {
            if (credit.creditor == "" || credit.debtor == "")
            {
                return false;
            }

            // credit amount must be positive
            if (credit.amount <= 0)
            {
                return false;
            }

            // creditor and debtor cannot be the same
            if (credit.creditor == credit.debtor)
            {
                return false;
            }

            // duplicate credits are not allowed
            for (int i = 0; i < this._unprocessedCredits.Count; i++)
            {
                if (this._unprocessedCredits[i].hash == credit.hash)
                {
                    return false;
                }
            }

            // duplicate credits are not allowed
            for (int i = 0; i < this._blockChain.Count; i++)
            {
                if (this._blockChain[i].credit.hash == credit.hash)
                {
                    return false;
                }
            }     

            return true;
        }

        public bool ProcessCredits()
        {            

            // mine a new block
            Credit minedCredit = new Credit("mined", "lender", 500);
            Block minedBlock = this.CreateBlock(minedCredit, this._blockChain[this._blockChain.Count - 1].hash);
            this._blockChain.Add(minedBlock);

            // if there are no unprocessed credits, return
            if (this._unprocessedCredits.Count == 0)
            {
                return true;
            }
            
            // if there are credits to process, process them
            foreach (Credit credit in this._unprocessedCredits)
            {
                Block newBlock = this.CreateBlock(credit, this._blockChain[this._blockChain.Count - 1].hash);                
                this.AddBlock(newBlock);
            }

            // clear the unprocessed credits
            this._unprocessedCredits.Clear();

            // assign keys to existing group creditors
            if (this._groupCreditors.Count > 0)
            {
                foreach (GroupCreditor group in this._groupCreditors)
                {
                    if (group.GetBalance() >= group.totalAmount)
                    {
                        group.AssignProofOfClaim();
                    }
                }
            }            

            return true;
        }

        // add new person to the group
        public bool AddPersonToGroup(string groupName, string personName)
        {
            if (this._groupCreditors.Exists(x => x.groupName == groupName))
            {
                GroupCreditor group = this._groupCreditors.Find(x => x.groupName == groupName);

                // if the person does not exist in the group, and the balance is not complete add them
                if (!group.people.Exists(x => x.name == personName) && group.GetBalance() <= group.totalAmount) 
                {
                    Person person = new Person(personName);
                    person.amount = group.totalAmount * (group.agreedProportion / 100);

                    Credit credit = new Credit(personName, groupName, person.amount);
                    credit.hash = credit.CalculateHash();
                    this._unprocessedCredits.Add(credit);

                    group.people.Add(person);
                    return true;
                }
            }

            return false;
        }

        public bool AddNewCredit(string creditor, float amount)
        {
            Credit credit = new Credit("lender", creditor, amount);
            credit.hash = credit.CalculateHash();

            if (IsValidNewCredit(credit))
            {
                this._unprocessedCredits.Add(credit);
                return true;
            }

            return false;
        }

        public string DisplayBlocks()
        {
            string output = "";

            output += "\nThe credit blocks are: \n";
            output += "The Smart Credit Blockchain is: " + (this.IsValidChain()? "valid" : "invalid") + "\n";

            foreach (Block block in this._blockChain)
            {
                int index = block.index + 1;
                output += "Block #" + index + "\n";
                output += "Previous Hash: " + block.previousHash + "\n";
                output += "Hash: " + block.hash + "\n";
                output += "TimeStamp: " + block.timeStamp + "\n";
                output += "Credit:\n";
                output += " Creditor: " + block.credit.creditor + "\n";
                output += " Debtor: " + block.credit.debtor + "\n";
                output += " Amount: " + block.credit.amount + "\n";
                output += "-------------------------------\n";
            }
            
            return output;
        }

        public string DisplayGroupCreditors()
        {
            string output = "";
            int Count = 0;
            foreach (GroupCreditor group in this._groupCreditors)
            {
                output += "Group Index: " + Count + "\n";
                output += "Name: " + group.groupName + "\n";
                output += "Total Amount: " + group.totalAmount + "\n";
                output += "Agreed Proportion: " + group.agreedProportion + "\n";
                output += "People:\n";
                foreach (Person person in group.people)
                {
                    output += " Name: " + person.name + "\n";
                    output += " Amount: " + person.amount + "\n";
                    output += " Proof of Claim: " + person.proofOfClaim + "\n";
                    output += " TimeStamp: " + person.timeStamp + "\n";
                    output += "-------------------------------\n";
                }
            }
            return output;
        }

        public string DisplayUnprocessedCredits()
        {
            string output = "";
            foreach (Credit credit in this._unprocessedCredits)
            {
                output += "Credit:\n";
                output += " Creditor: " + credit.creditor + "\n";
                output += " Debtor: " + credit.debtor + "\n";
                output += " Amount: " + credit.amount + "\n";
                output += " TimeStamp: " + credit.timeStamp + "\n";
                output += "-------------------------------\n";
            }
            return output;
        }

        public string DisplayProcessedCredits()
        {
            string output = "";
            foreach (Block block in this._blockChain)
            {
                output += "Credit:\n";
                output += " Creditor: " + block.credit.creditor + "\n";
                output += " Debtor: " + block.credit.debtor + "\n";
                output += " Amount: " + block.credit.amount + "\n";
                output += " TimeStamp: " + block.credit.timeStamp + "\n";
                output += "-------------------------------\n";
            }
            return output;
        }
    }
}
