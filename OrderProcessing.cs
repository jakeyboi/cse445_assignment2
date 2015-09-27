﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

// Also need send a notification back to TravelAgency when order is completed
// probably just fire an event back when it's completed


//TODO: create event to fire back once order is completed

namespace Assignment2
{
    class OrderProcessing
    {
        private OrderClass order;
        private const double TAX = 1.1;
        private const double LOCATION_CHARGE = 10.00;

        // process the order here
        public OrderProcessing(OrderClass order)
        {
            this.order = order;
        }

        public void ProcessNewOrder()
        {
            if (CardIsValid(order.GetCardNo()))
            {
                double totalCost = (order.GetUnitPrice() * order.GetAmount() + LOCATION_CHARGE) * TAX;
                Console.WriteLine("Total cost of order for " + order.GetReceiverId() + " from " + order.GetSenderId() + " is " + totalCost + " at unit price " + order.GetUnitPrice() + ".");

                String temp = order.GetReceiverId();
                order.SetReceiverId(order.GetSenderId());
                order.SetSenderId(temp);

                // Encode confirmation
                String confStr = Encoder.EncodeOrder(order);

                // Write to confirmationBuffer
                Console.WriteLine("CONF BUFFER WAIT ONE, ORDER PROCESSING, from travel agency " + order.GetSenderId() + " to " + order.GetReceiverId());
                MainProgram.confirmationBuffer.sem.WaitOne();
               
                MainProgram.confirmationBuffer.SetOneCell(confStr);
                Console.WriteLine(order.GetSenderId() + " has sent confirmation to " + order.GetReceiverId() + " for order of unit price " + order.GetUnitPrice() + ".");
            }
        }

        // credit card # must be between 5000 and 7000
        private bool CardIsValid(int cardNo)
        {
            return (cardNo >= 5000 && cardNo <= 7000);
        }
    }
}
