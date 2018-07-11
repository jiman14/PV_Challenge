// -----------------------------------------------------------------------
// <copyright file="FraudRadar.cs" company="Payvision">
//     Payvision Copyright © 2017
// </copyright>
// -----------------------------------------------------------------------

namespace Payvision.CodeChallenge.Refactoring.FraudDetection
{
    using System;
    using System.Collections.Generic;

    public class FraudRadar
    {
        #region Vars and FraudRadar constructor

        private List<Order> orders;             // Valid order list 
        private List<Order> invalidOrders;      // Invalid order list 
        private List<Order> fraudOrders;        // Fraudulents order list                         
        private Logger logger;                  // Logger for info and errors

        private string[] txtData;               // Data lines to import

        /// <summary>
        /// Initialize object and load txt file data
        /// </summary>
        public FraudRadar()
        {
            logger = new Logger();

            // Initialize orders arrays
            orders = new List<Order>();
            invalidOrders = new List<Order>();
            fraudOrders = new List<Order>();
        }
        #endregion

        #region Load orders

        /// <summary>
        /// Set TXT Data 
        /// </summary>
        /// <param name="_txtData_Lines"></param>
        public void SetTXTData(string[] _txtData_Lines)
        {
            // check if null set empty string
            if (_txtData_Lines == null)
                _txtData_Lines = new string [0];

            txtData = _txtData_Lines;
        }

        /// <summary>
        /// Get orders from file and insert in List
        /// </summary>
        /// <param name="filePath"></param>
        public void LoadOrders()
        {
            // set active order in logger
            logger.AddInfo("Load data process begins...");
            try
            {
                // insert one order per line
                for (int i = 0; i < txtData.Length; i++)
                {
                    // logger active order id
                    logger.setActiveOrder(i);

                    // Initialize order class with field desc
                    Order tempOrder = new Order(ref logger);

                    logger.AddInfo("Mapping a string into order's object. Line nº: " + i.ToString());

                    // Mapping a string into order's object
                    tempOrder.LoadOrder(i, txtData[i]);

                    if (!tempOrder.hasErrors)
                    {
                        if (!checkFraud(tempOrder))
                        {
                            // Add valid order
                            orders.Add(tempOrder);
                        }
                        else
                        {
                            // Add fraudulent order
                            fraudOrders.Add(tempOrder);
                            logger.AddInfo("Fraudulent detection in line nº: " + i.ToString());
                        }
                    }
                    else
                    {
                        // Add invalid order
                        invalidOrders.Add(tempOrder);
                        logger.AddInfo("Invalid data detection in line nº: " + i.ToString());
                    }
                }
            }
            catch (Exception exc)
            {
                // set active order in logger
                logger.LogNoOrderInfo();
                logger.AddException(exc);
            }

            // set active order in logger
            logger.LogNoOrderInfo();
            logger.AddInfo(string.Format("Ends of orders load process. Valid orders: {0}. Invalid orders: {1}. Fraudulents orders: {2} ",
                orders.Count.ToString(), invalidOrders.Count.ToString(), fraudOrders.Count.ToString()));
        }

        #endregion

        #region Fraud detection

        /// <summary>
        /// Check frauds by comparision with stored valid orders
        /// </summary>
        /// <param name="current"></param>
        /// <returns></returns>
        private bool checkFraud(Order current)
        {
            bool isFraudulent = false;

            // valid orders bucle
            foreach (Order order in orders)
            {
                // If the DealId and Email fields are equals, CreditCard must be equals too
                if (current.DealId == order.DealId
                    && current.Email == order.Email
                    && current.CreditCard != order.CreditCard)
                {
                    isFraudulent = true;
                }

                // If the DealId field and Postal data are equals, CreditCard must be equals too
                if (current.DealId == order.DealId
                    && current.State == order.State
                    && current.ZipCode == order.ZipCode
                    && current.Street == order.Street
                    && current.City == order.City
                    && current.CreditCard != order.CreditCard)
                {
                    isFraudulent = true;
                }

                // Insert into FraudResult list
                if (isFraudulent)
                {
                    // Set fraudulent order flag                    
                    current.IsFraudulent = true;
                    break;
                }
            }
            return isFraudulent;
        }
        #endregion

        #region Public properties for get: valid, invalid & fraudulents orders

        /// <summary>
        /// Get fraudulents orders
        /// </summary>
        public List<Order> getFraudsOrders
        {
            get { return fraudOrders; }
        }
        /// <summary>
        /// Get valid orders
        /// </summary>
        public List<Order> getValidOrders
        {
            get { return orders; }
        }
        /// <summary>
        /// Get invalids orders (data corrupted or string malformed)
        /// </summary>
        public List<Order> getInvalidOrders
        {
            get { return invalidOrders; }
        }

        #endregion

        #region Manage Order 

        /// <summary>
        /// Field metadata
        /// </summary>
        public class OrderField
        {
            public string fieldName;                    // name of the field
            public DataCheck.ChecksTypes checkType;     // cheks type that will pass
            public System.Type dataType;                // Data type
            public bool mandatory;                      // Required data 
            public object value = null;                 // Object value
        }

        /// <summary>
        /// Manage order
        /// </summary>
        public class Order
        {
            private Logger logger;              // Logger var
            List<OrderField> fieldDesc;         // List of order's fields
            private DataCheck check;            // Checking class

            /// <summary>
            /// Initialize an order with fieldDesc class injection
            /// </summary>
            /// <param name="_logger"></param>
            /// <param name="_fieldDesc"></param>
            public Order(ref Logger _logger)
            {
                logger = _logger;
                check = new DataCheck(ref logger);

                // Load fields 
                InitializefieldDesc();
            }
            
            // todo: automatize this description load
            /// <summary>
            /// Initialize fields description manually 
            /// </summary>
            private void InitializefieldDesc()
            {
                fieldDesc = new List<OrderField>();

                fieldDesc.Add(new OrderField()
                {
                    fieldName = "OrderId",
                    checkType = DataCheck.ChecksTypes.PositiveInteger,
                    dataType = typeof(Int32),
                    mandatory = true
                });

                fieldDesc.Add(new OrderField()
                {
                    fieldName = "DealId",
                    checkType = DataCheck.ChecksTypes.PositiveInteger,
                    dataType = typeof(Int32),
                    mandatory = true
                });

                fieldDesc.Add(new OrderField()
                {
                    fieldName = "Email",
                    checkType = DataCheck.ChecksTypes.Email,
                    dataType = typeof(Int32),
                    mandatory = true
                });

                fieldDesc.Add(new OrderField()
                {
                    fieldName = "Street",
                    checkType = DataCheck.ChecksTypes.Street,
                    dataType = typeof(Int32),
                    mandatory = true
                });

                fieldDesc.Add(new OrderField()
                {
                    fieldName = "City",
                    checkType = DataCheck.ChecksTypes.String,
                    dataType = typeof(Int32),
                    mandatory = true
                });

                fieldDesc.Add(new OrderField()
                {
                    fieldName = "State",
                    checkType = DataCheck.ChecksTypes.State,
                    dataType = typeof(Int32),
                    mandatory = true
                });

                fieldDesc.Add(new OrderField()
                {
                    fieldName = "ZipCode",
                    checkType = DataCheck.ChecksTypes.NO_CHECK,
                    dataType = typeof(Int32),
                    mandatory = true
                });

                fieldDesc.Add(new OrderField()
                {
                    fieldName = "CreditCard",
                    checkType = DataCheck.ChecksTypes.NO_CHECK,
                    dataType = typeof(Int32),
                    mandatory = true
                });
            }

            /// <summary>
            /// Map order from string, performs check and normalizations 
            /// </summary>
            /// <param name="idOrder"></param>
            /// <param name="orderLine"></param>
            public void LoadOrder(int idOrder, string orderLine)
            {
                string[] orderParts = orderLine.Split(',');
                
                if (orderParts.Length != fieldDesc.Count)
                {
                    logger.AddInvalidDataError("Order line malformed, item counts: " + orderParts.Length);
                }
                else
                {                    
                    // Check all data, to know all the errors at the end of the mapping process 
                    for (int i=0; i < fieldDesc.Count; i++)
                    {
                        check.FieldDataCheck(fieldDesc[i], orderParts[i]);                                                
                    }
                }
            }

            #region Public properties: field mapping by name

            // Map fields by name

            public int OrderId { get { return (int)fieldDesc[0].value; } }

            public int DealId { get { return (int)fieldDesc[1].value; } }

            public string Email { get { return (string)fieldDesc[2].value; } }

            public string Street { get { return (string)fieldDesc[3].value; } }

            public string City { get { return (string)fieldDesc[4].value; } }

            public string State { get { return (string)fieldDesc[5].value; } }

            public string ZipCode { get { return (string)fieldDesc[6].value; } }

            public string CreditCard { get { return (string)fieldDesc[7].value; } }

            public bool IsFraudulent { get; set; }

            #endregion

            /// <summary>
            /// Return if there is invalid data in 
            /// </summary>
            public bool hasErrors
            {
                get { return logger.thereisInvalidData; }
            }
        }

        #endregion

        #region Utilities


        /// <summary>
        /// Logger class for log & exceptions 
        /// </summary>
        public class Logger
        {
            internal enum LogType
            {
                INVALID_DATA,
                EXCEPTION,
                INFO
            }           // Log's record type

            // Log item struct: type and message
            internal struct LogItem
            {
                public LogType logType;
                public string message;
            }

            // Log dictionary to facilite access by orderId
            private Dictionary<int, List<LogItem>> Log;

            // active orderID
            private int activeOrderId;

            // No order related info & errors
            public const int LOG_NO_ORDER_INFO = -1;

            /// <summary>
            /// Initialize logger
            /// </summary>
            public Logger()
            {
                Log = new Dictionary<int, List<LogItem>>();

                // Initialize active order id
                LogNoOrderInfo();
            }

            /// <summary>
            /// Log info & errors with no order id relation
            /// </summary>
            public void LogNoOrderInfo()
            {
                activeOrderId = LOG_NO_ORDER_INFO;
            }

            /// <summary>
            /// Get order log by id (create if not exist)
            /// </summary>
            /// <param name="orderId"></param>
            /// <returns></returns>
            internal List<LogItem> getOrderLog(int orderId)
            {
                if (!Log.ContainsKey(activeOrderId))
                    Log.Add(activeOrderId, new List<LogItem>());

                return Log[activeOrderId];
            }

            /// <summary>
            /// Set active order by orderId
            /// </summary>
            /// <param name="orderId"></param>
            internal void setActiveOrder(int orderId)
            {
                activeOrderId = orderId;
            }

            /// <summary>
            /// Add info to log
            /// </summary>
            /// <param name="orderId"></param>
            /// <param name="message"></param>
            public void AddInfo(string message)
            {
                getOrderLog(activeOrderId).Add(new LogItem()
                {
                    logType = LogType.INFO,
                    message = message
                });
            }

            /// <summary>
            /// Add exception to log
            /// </summary>
            /// <param name="orderId"></param>
            /// <param name="exception"></param>
            public void AddException(Exception exception)
            {
                getOrderLog(activeOrderId).Add(new LogItem()
                {
                    logType = LogType.EXCEPTION,
                    message = exception.Message
                });
            }

            /// <summary>
            /// Add invalid data error to log
            /// </summary>
            /// <param name="orderId"></param>
            /// <param name="message"></param>
            public void AddInvalidDataError(string message)
            {
                getOrderLog(activeOrderId).Add(new LogItem()
                {
                    logType = LogType.INVALID_DATA,
                    message = message
                });
            }

            /// <summary>
            /// Check if there is exceptions in order
            /// </summary>
            /// <param name="orderId"></param>
            /// <returns></returns>
            public bool thereisExceptions
            {
                get
                {
                    return getOrderLog(activeOrderId).Exists(i => i.logType == LogType.EXCEPTION);
                }
            }

            /// <summary>
            /// Check if thereis invalid data in order
            /// </summary>
            /// <param name="orderId"></param>
            /// <returns></returns>
            public bool thereisInvalidData
            {
                get
                {
                    return getOrderLog(activeOrderId).Exists(i => i.logType == LogType.INVALID_DATA);
                }
            }

        }

        /// <summary>
        /// Utility class for checks and normalize data
        /// Contains a batery of check functions
        /// </summary>
        public class DataCheck
        {
            /// <summary>
            /// List of avaiable checks
            /// </summary>
            public enum ChecksTypes
            {
                String,
                PositiveInteger,
                Email,
                Street,
                State,
                NO_CHECK
            }
            
            Normalize normalize;        // Normalize 
            Logger logger;              // logger 

            /// <summary>
            /// Initialize data check
            /// </summary>
            /// <param name="_logger"></param>
            public DataCheck(ref Logger _logger)
            {
                logger = _logger;
                normalize = new Normalize();
            }

            /// <summary>
            /// Check data generic function based on OrderfieldDesc
            /// </summary>
            /// <param name="datafield"></param>
            /// <param name="value"></param>
            public void FieldDataCheck(OrderField datafield, string value)
            {
                if (datafield.checkType == ChecksTypes.PositiveInteger)
                {
                    PositiveInteger(datafield, value);                    
                }
                else if (datafield.checkType == ChecksTypes.Email)
                {
                    Email(datafield, value);
                }
                else if (datafield.checkType == ChecksTypes.State)
                {
                    State(datafield, value);
                }
                else if (datafield.checkType == ChecksTypes.Street)
                {
                    Street(datafield, value);
                }
                else if (datafield.checkType == ChecksTypes.String)
                {
                    ChkString(datafield, value);
                }
                else
                {
                    // Set value without check
                    datafield.value = value;
                }
            }

            #region Check functions

            /// <summary>
            /// Check if positive integer
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            private void PositiveInteger(OrderField dataField, string value)
            {
                int temp = 0;
                if (!int.TryParse(value, out temp))
                {
                    logger.AddInvalidDataError(string.Format("Invalid {0}: is not an integer", dataField.fieldName.ToString()));
                }
                else if (temp < 0)
                {
                    logger.AddInvalidDataError(string.Format("Invalid {0}: is not positive integer", dataField.fieldName.ToString()));
                }     
                else
                {
                    dataField.value = temp;
                }
            }

            /// <summary>
            /// Generic String check
            /// </summary>
            /// <param name="dataField"></param>
            /// <param name="value"></param>
            private void ChkString(OrderField dataField, string value)
            {
                value = value.Trim().ToLower();
                if ((dataField.mandatory) && (value == ""))
                {
                    logger.AddInvalidDataError(string.Format("Invalid {0}: empty string", dataField.fieldName.ToString()));
                }
                else
                {
                    dataField.value = value;
                }
            }

            /// <summary>
            /// Check email format _____@___.__ and normalize
            /// </summary>
            /// <param name="Email"></param>
            /// <returns></returns>
            private void Email(OrderField dataField, string Email)
            {
                // Check basic string 
                ChkString(dataField, Email);
                bool invalidFormat = (dataField.value.ToString() == "");

                // Check email contains @
                if (!invalidFormat)
                {
                    Email = dataField.value.ToString();
                    invalidFormat = (Email.IndexOf("@") <= 0);
                }

                // Check email contains only one @
                if (!invalidFormat)
                {
                    invalidFormat = (Email.Split(new char[] { '@' }).Length != 2);
                }

                // Check email contains @ before a "."
                if (!invalidFormat)
                {
                    invalidFormat = (Email.LastIndexOf(".") < Email.IndexOf("@"));
                }
                
                // Save output value if not invalid
                if (!invalidFormat)
                {
                    dataField.value = normalize.Email(Email);
                }
                else
                {
                    logger.AddInvalidDataError("Invalid email format");
                }
            }

            /// <summary>
            /// Check and normalize State
            /// </summary>
            /// <param name="dataField"></param>
            /// <param name="value"></param>
            private void State(OrderField dataField, string value)
            {
                ChkString(dataField, value);
                if (dataField.value.ToString() != "")
                    dataField.value = normalize.State(dataField.value.ToString());
            }

            /// <summary>
            /// Check and normalize Street
            /// </summary>
            /// <param name="dataField"></param>
            /// <param name="value"></param>
            private void Street(OrderField dataField, string value)
            {
                ChkString(dataField, value);
                if (dataField.value.ToString() != "")
                    dataField.value = normalize.Street(dataField.value.ToString());
            }

            #endregion

            /// <summary>
            /// Get data errors
            /// </summary>
            public bool ThereIsErrors
            {
                get { return logger.thereisInvalidData; }
            }
        }

        /// <summary>
        /// Utility class for normalize
        /// </summary>
        public class Normalize
        {
            /// <summary>
            /// Normalize email, format required ___@___.__ (previously checked)
            /// </summary>
            /// <param name="Email"></param>
            /// <returns></returns>
            public string Email(string Email)
            {                
                var aux = Email.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);

                if (aux.Length > 1)
                {
                    var atIndex = aux[0].IndexOf("+", StringComparison.Ordinal);

                    aux[0] = atIndex < 0 ? aux[0].Replace(".", "") : aux[0].Replace(".", "").Remove(atIndex);

                    return string.Join("@", new string[] { aux[0], aux[1] });
                }
                else return aux[0];
            }
            /// <summary>
            /// Normalize street
            /// </summary>
            public string Street(string Street)
            {
                Dictionary<string, string> replaces = new Dictionary<string, string>();
                replaces.Add("st.", "street");
                replaces.Add("rd.", "road");

                foreach (string replace in replaces.Keys)
                {
                    Street = Street.Replace(replace, replaces[replace]);
                }

                return Street;
            }
            /// <summary>
            /// Normalize state
            /// </summary>
            /// <param name="State"></param>
            /// <returns></returns>
            public string State(string State)
                    {
                        Dictionary<string, string> replaces = new Dictionary<string, string>();
                        replaces.Add("il", "illinois");
                        replaces.Add("ca", "california");
                        replaces.Add("ny", "new york");

                        foreach (string replace in replaces.Keys)
                        {
                            State = State.Replace(replace, replaces[replace]);
                        }

                        return State;
                    }
        }

        #endregion
    }
}