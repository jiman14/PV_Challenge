// -----------------------------------------------------------------------
// <copyright file="FraudRadarTests.cs" company="Payvision">
//     Payvision Copyright © 2017
// </copyright>
// -----------------------------------------------------------------------

namespace Payvision.CodeChallenge.Refactoring.FraudDetection.Tests
{
    using System;
    using System.IO;
    using System.Linq;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class FraudRadarTests
    {
        FraudRadar fraudRadar = null;

        #region Test files declaration

        private const string F_TestFolder = "Files";
        private const string F_OneLineFile = "./Files/OneLineFile.txt";
        private const string F_TwoLines_FraudulentSecond = "./Files/TwoLines_FraudulentSecond.txt";
        private const string F_ThreeLines_FraudulentSecond = "./Files/ThreeLines_FraudulentSecond.txt";
        private const string F_FourLines_MoreThanOneFraudulent = "./Files/FourLines_MoreThanOneFraudulent.txt";
        #endregion

        /// <summary>
        /// Initialize FraudRadar, load txt file and process orders
        /// </summary>
        /// <param name="txt_file"></param>
        private void FraudRadarInitialize(string txt_file)
        {
            // Set file path
            txt_file = Path.Combine(Environment.CurrentDirectory, txt_file);
            
            // Check if file exists
            if (File.Exists(txt_file))
            {
                // Initialize FraudRadar
                fraudRadar = new FraudRadar();

                // Set data from file
                fraudRadar.SetTXTData(File.ReadAllLines(txt_file));

                // Load orders from data
                fraudRadar.LoadOrders();
            }
            else
            {
                // Throw exception if file not exits
                throw new FileNotFoundException("File not found");
            }
        }

        [TestMethod]
        [DeploymentItem(F_OneLineFile, F_TestFolder)]
        public void CheckFraud_OneLineFile_NoFraudExpected()
        {
            FraudRadarInitialize(F_OneLineFile);
            var result = fraudRadar.getFraudsOrders;

            result.Should().NotBeNull("The result should not be null.");
            result.Count().ShouldBeEquivalentTo(0, "The result should not contains fraudulent lines");
        }

        [TestMethod]
        [DeploymentItem(F_TwoLines_FraudulentSecond, F_TestFolder)]
        public void CheckFraud_TwoLines_SecondLineFraudulent()
        {
            FraudRadarInitialize(F_TwoLines_FraudulentSecond);
            var result = fraudRadar.getFraudsOrders;

            result.Should().NotBeNull("The result should not be null.");
            result.Count().ShouldBeEquivalentTo(1, "The result should contains the number of lines of the file");
            result.First().IsFraudulent.Should().BeTrue("The first line is not fraudulent");
            result.First().OrderId.Should().Be(2, "The first line is not fraudulent");
        }

        [TestMethod]
        [DeploymentItem(F_ThreeLines_FraudulentSecond, F_TestFolder)]
        public void CheckFraud_ThreeLines_SecondLineFraudulent()
        {
            FraudRadarInitialize(F_ThreeLines_FraudulentSecond);
            var result = fraudRadar.getFraudsOrders;

            result.Should().NotBeNull("The result should not be null.");
            result.Count().ShouldBeEquivalentTo(1, "The result should contains the number of lines of the file");
            result.First().IsFraudulent.Should().BeTrue("The first line is not fraudulent");
            result.First().OrderId.Should().Be(2, "The first line is not fraudulent");
        }

        [TestMethod]
        [DeploymentItem(F_FourLines_MoreThanOneFraudulent, F_TestFolder)]
        public void CheckFraud_FourLines_MoreThanOneFraudulent()
        {
            FraudRadarInitialize(F_FourLines_MoreThanOneFraudulent);
            var result = fraudRadar.getFraudsOrders;

            result.Should().NotBeNull("The result should not be null.");
            result.Count().ShouldBeEquivalentTo(2, "The result should contains the number of lines of the file");
        }
        
    }
}