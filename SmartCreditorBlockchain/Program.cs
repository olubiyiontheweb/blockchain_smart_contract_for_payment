using System;

namespace SmartCreditorBlockchain
{
    internal class Program
    {
        // intialize the credit blockchain producer
        static CreditBlockProducer creditBlockProducer = new CreditBlockProducer();

        static void Main(string[] args)
        {            
            Console.WriteLine("\nWelcome to SmartCreditorBlockchain!\n");
            
            while (true)
            {
                try
                {
                    prompter();
                }
                catch (System.Exception)
                {                    
                    Console.WriteLine("\nSomething went wrong. Please try again.\n");
                    prompter();
                }       
            }
        }

        private static void prompter()
        {
            Console.WriteLine("\nWhat do you want to do? Type a letter and press enter\n");
            Console.WriteLine("A: Fund a new creditor");
            Console.WriteLine("B: Check the balance of a creditor");
            Console.WriteLine("C: Display the credit blocks");
            Console.WriteLine("D: Display unprocessed credit transactions");
            Console.WriteLine("E: Process unprocessed credit transactions");
            Console.WriteLine("F: Display all processed credit transactions in the blockchain");
            Console.WriteLine("G: Create a new credit group");
            Console.WriteLine("H: Display the credit groups");
            Console.WriteLine("I: Add a creditor to a credit group");
            Console.WriteLine("J: Exit");
            string action = Console.ReadLine();

            switch (action.ToLowerInvariant())
            {
                case "a":
                    Console.WriteLine("\nPlease enter the name of the creditor you want to fund");
                    string creditorName = Console.ReadLine();
                    Console.WriteLine("Please enter the amount of money you want to give to the creditor");
                    string amount = Console.ReadLine();
                    
                    if (creditBlockProducer.AddNewCredit(creditorName, float.Parse(amount)))
                    {
                        Console.WriteLine("\nCredit transaction added successfully!");
                    }
                    else
                    {
                        Console.WriteLine("\nCredit transaction failed!");
                    }
                    
                    break;

                case "b":
                    Console.WriteLine("\nPlease enter the name of the creditor you want to check the balance of");
                    string creditorName2 = Console.ReadLine();
                    Console.WriteLine(creditBlockProducer.GetBalance(creditorName2));
                    break;

                case "c":                   
                    Console.WriteLine(creditBlockProducer.DisplayBlocks());
                    break;

                case "d":
                    Console.WriteLine("\nThe unprocessed credit transactions are: \n");
                    Console.WriteLine(creditBlockProducer.DisplayUnprocessedCredits());
                    break;

                case "e":
                    Console.WriteLine("\nProcessing unprocessed credit transactions .....");

                    if(creditBlockProducer.ProcessCredits())
                    {
                        Console.WriteLine("\nAll unprocessed credit transactions have been processed successfully!");
                    }
                    else
                    {
                        Console.WriteLine("\nProcessing of unprocessed credit transactions failed!");
                    }
                    
                    break;

                case "f":
                    // display all processed credit transactions in the blockchain
                    Console.WriteLine("\nThe processed credit transactions are: \n");
                    Console.WriteLine(creditBlockProducer.DisplayProcessedCredits());
                    break;

                case "g":
                    Console.WriteLine("\nPlease enter the name of the credit group you want to create");
                    string groupName = Console.ReadLine();
                    Console.WriteLine("Please enter the total amount of money the group is giving as credit");
                    string totalAmount = Console.ReadLine();
                    Console.WriteLine("Please enter the agreed sharing proportion for the group members");
                    string agreedProportion = Console.ReadLine();
                    
                    if(creditBlockProducer.CreateNewGroup(groupName, float.Parse(totalAmount), float.Parse(agreedProportion)))
                    {
                        Console.WriteLine("\nCredit group created successfully!");
                    }
                    else
                    {
                        Console.WriteLine("\nCredit group creation failed!");
                    }
                    
                    break;

                case "h":
                    Console.WriteLine("\nThe credit groups are: \n");
                    Console.WriteLine(creditBlockProducer.DisplayGroupCreditors());
                    break;
                case "i":
                    Console.WriteLine("\nPlease enter the name of the creditor you want to add to the credit group");
                    string creditorName3 = Console.ReadLine();
                    Console.WriteLine("Please enter the name of the credit group you want to add the creditor to");
                    string groupName2 = Console.ReadLine();
                    
                    if(creditBlockProducer.AddPersonToGroup(groupName2, creditorName3))
                    {
                        Console.WriteLine("\nCreditor added to credit group successfully!");
                    }
                    else
                    {
                        Console.WriteLine("\nCreditor addition to credit group failed!");
                    }

                    break;
                case "j":
                    Console.WriteLine("\nExiting SmartCreditorBlockchain....");
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("\nInvalid input\n");
                    prompter();
                    break;
            }
        }
    }
}
