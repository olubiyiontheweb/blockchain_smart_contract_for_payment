using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace SmartCreditorBlockchain
{
    public class Block {
        public string hash { get; set; }
        public string previousHash { get; set; }
        public Credit credit { get; set; }
        public DateTime timeStamp { get; set; }
        private int nonce { get; set; }
        public int index { get; set; }

        public Block(string previousHash, Credit credit) {
            this.previousHash = previousHash;
            this.credit = credit;
            this.timeStamp = DateTime.Now;
            this.nonce = 0;
        }

        public string CalculateHash() {
            string stringdata = previousHash.ToString() + credit.ToString() + timeStamp.ToString() + nonce.ToString() + index.ToString();
            SHA256 sha256 = SHA256.Create();
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(stringdata));
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++) {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }

        // proof of work
        public void MineBlock(int difficulty) {
            string target = new string('0', difficulty);
            string hashTemp = this.CalculateHash();
            this.hash = hashTemp;

            while (hashTemp.Substring(0, difficulty) != target) {
                this.nonce++;
                hashTemp = CalculateHash();
            }
            this.nonce = 0;
        }
    }

    public class Credit {
        public string creditor { get; set; }
        public string debtor { get; set; }
        public float amount { get; set; }
        public DateTime timeStamp { get; set; }
        public string hash { get; set; }

        public Credit(string creditor, string debtor, float amount) {
            this.creditor = creditor;
            this.debtor = debtor;
            this.amount = amount;
            this.timeStamp = DateTime.Now;
        }

        public string CalculateHash() {
            string stringdata = this.creditor + this.debtor + this.amount + this.timeStamp.ToString();
            byte[] dataBytes = System.Text.Encoding.UTF8.GetBytes(stringdata);
            byte[] hashBytes = new System.Security.Cryptography.SHA256Managed().ComputeHash(dataBytes);
            return Convert.ToBase64String(hashBytes);
        }
    }

    public class GroupCreditor {
        public string groupName { get; set; }
        public float totalAmount { get; set; }
        public List<Person> people { get; set; }
        public DateTime timeStamp { get; set; }

        public float agreedProportion { get; set; }
        public string hash { get; set; }

        public GroupCreditor(string groupName, float totalAmount, float agreedProportion) {
            this.groupName = groupName;
            this.totalAmount = totalAmount;
            this.agreedProportion = agreedProportion;
            this.people = new List<Person>();
            this.timeStamp = DateTime.Now;
            this.hash = CalculateHash();
        }

        public string CalculateHash() {
            string stringdata = groupName + totalAmount + timeStamp.ToString() + agreedProportion;
            byte[] dataBytes = System.Text.Encoding.UTF8.GetBytes(stringdata);
            byte[] hashBytes = new System.Security.Cryptography.SHA256Managed().ComputeHash(dataBytes);
            return Convert.ToBase64String(hashBytes);
        }
        
        public float GetBalance() {
            float balance = 0;
            foreach (Person person in this.people) {
                balance += person.amount;
            }
            return balance;
        }

        // assign public key to each person
        public void AssignProofOfClaim() {
            foreach (Person person in this.people) {
                // public key is assigned to each person
                person.assignKeys();
            }
        }
    }

    public class Person {
        public string name { get; set; }
        public float amount { get; set; }
        public DateTime timeStamp { get; set; }
        private string privateKey { get; set; }

        // public key
        public string proofOfClaim { get; set; }

        public Person(string name) {
            this.name = name;
            this.amount = amount;
            this.timeStamp = DateTime.Now;
        }

        public void assignKeys() {
            // generate key pair
            RSA rsa = RSA.Create();  
            // assign private key
            this.privateKey = rsa.ToXmlString(true);
            // assign public key
            this.proofOfClaim = rsa.ToXmlString(false);
        }
    }
}