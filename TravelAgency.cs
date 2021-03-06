﻿using System;
using System.Threading;

namespace Assignment2
{
    class TravelAgency
    {
        // Track current price to compare with new prices
        private int currentPrice;
        // Boolean indicating whether to place an order
        private bool placeOrder;
        // Amount of tickets to order if placeOrder is true
        private OrderClass orderToBePlaced;

        // Constructor
        public TravelAgency()
        {
            currentPrice = 500;
            placeOrder = false;
            orderToBePlaced = null;
        }

        // Thread method
        public void TravelAgencyFunc()
        {
            while (MainProgram.airlineThreadCount > 0)
            {
                // Check if an order should be placed
                if (placeOrder)
                {                  
                    orderToBePlaced.SetSenderId(Thread.CurrentThread.Name);
                    orderToBePlaced.SetTimestamp(DateTime.Now);

                    // print order to the console
                    Console.WriteLine(orderToBePlaced.GetSenderId() + " has placed an order for " + orderToBePlaced.GetAmount() 
                    + " tickets for " + orderToBePlaced.GetUnitPrice() + " each at " + orderToBePlaced.GetTimestamp());

                    // Encode the order for transmission
                    String orderStr = Encoder.EncodeOrder(orderToBePlaced);

                    // Write to buffer
                    MainProgram.orderBuffer.sem.WaitOne();
                    MainProgram.orderBuffer.SetOneCell(orderStr);

                    // Update current price
                    currentPrice = orderToBePlaced.GetUnitPrice();
                    placeOrder = false;
                }

                // Read a confirmation from the buffer
                String confString = MainProgram.confirmationBuffer.GetOneCell(Thread.CurrentThread.Name);

                // If a confirmation was received
                if (confString != null)
                {
                    MainProgram.confirmationBuffer.sem.Release(1);
                    OrderClass order = Decoder.DecodeOrder(confString);
                    Console.WriteLine(Thread.CurrentThread.Name + " received order confirmation from " + order.GetSenderId() + " for " 
                        + order.GetAmount() + " at " + order.GetUnitPrice() + " each ordered at " + order.GetTimestamp());
                }
            }
        }

        // Event handler for the Airline to call when a price-cut event occurs
        public void TicketPriceCut(String airlineId, int newPrice)
        {
            if (!placeOrder)
            {
                int orderAmount = 0;
                int priceDifference = currentPrice - newPrice;

                // Only proceed if new price is less than current price
                if (priceDifference > 0)
                {
                    if (priceDifference < 100)
                    {
                        orderAmount = 5;
                    }
                    else if (priceDifference < 300)
                    {
                        orderAmount = 15;
                    }
                    else if (priceDifference < 500)
                    {
                        orderAmount = 25;
                    }
                    else if (priceDifference <= 800)
                    {
                        orderAmount = 35;
                    }

                    // Create a new order
                    orderToBePlaced = new OrderClass();
                    orderToBePlaced.SetCardNo(6000);
                    orderToBePlaced.SetReceiverId(airlineId);
                    orderToBePlaced.SetAmount(orderAmount);
                    orderToBePlaced.SetUnitPrice(newPrice);

                    placeOrder = true;
                }
            }
        }
    }
}