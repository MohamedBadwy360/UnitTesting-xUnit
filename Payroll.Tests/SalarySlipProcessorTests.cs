using Moq;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Payroll.Tests
{
    public class SalarySlipProcessorTests
    {
        //[Fact]
        //public void Method_Scenario_Outcome() { }

        [Fact]
        public void CalculateBasicSalary_EmployeesIsNull_ThrowArgumentNullException()
        {
            // Arrange
            Employee employee = null;

            // Act
            SalarySlipProcessor salarySlipProcessor = new SalarySlipProcessor(null);

            Func<Employee, decimal> func = (e) => salarySlipProcessor.CalculateBasicSalary(e);

            // Assert
            Assert.Throws<ArgumentNullException>(() => func(employee));
        }

        [Fact] 
        public void CalculateBasicSalary_ForEmployeeWageAndWorkingDays_ReturnsBasicSalary()
        {
            // Arrange
            var employee = new Employee { Wage = 500m, WorkingDays = 20 };

            // Act
            var salraySlipProcessor = new SalarySlipProcessor(null);

            var actual = salraySlipProcessor.CalculateBasicSalary(employee);

            var expected = 10000m;

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CalculateTransportationAllowece_EmployeeIsNull_ThrowArgumentNullException()
        {
            // Arrange
            Employee employee = null;

            // Act
            Func<Employee, decimal> func = (e) => new SalarySlipProcessor(null).CalculateTransportationAllowece(e);

            // Assert
            Assert.Throws<ArgumentNullException>(() => func(employee));
        }

        [Fact]
        public void CalculateTransportationAllowece_ForEmployeeWorkInOffice_ReturnsTransportationAllowece()
        {
            // Arrange
            Employee employee = new Employee { WorkPlatform = WorkPlatform.Office };

            // Act
            var actual = new SalarySlipProcessor(null).CalculateTransportationAllowece(employee);

            var expected = Constants.TransportationAllowanceAmount;

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CalculateTransportationAllowece_ForEmployeeWorkRemote_ReturnsTransportationAllowece()
        {
            // Arrange
            Employee employee = new Employee { WorkPlatform = WorkPlatform.Remote };

            // Act
            var actual = new SalarySlipProcessor(null).CalculateTransportationAllowece(employee);

            var expected = 0m;

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CalculateTransportationAllowece_ForEmployeeWorkHybrid_ReturnsTransportationAllowece()
        {
            // Arrange
            Employee employee = new Employee { WorkPlatform = WorkPlatform.Hybrid };

            // Act
            var actual = new SalarySlipProcessor(null).CalculateTransportationAllowece(employee);

            var expected = Constants.TransportationAllowanceAmount / 2;

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CalculateDangerPay_EmployeeIsNull_ReturnsArgumentNullException()
        {
            // Arrange
            Employee employee = null;

            // Act
            Func<Employee, decimal> func = (e) =>  new SalarySlipProcessor(null)
                                                        .CalculateDangerPay(e);
            // Assert
            Assert.Throws<ArgumentNullException>(() => func(employee));
        }

        [Fact]
        public void CalculateDangerPay_EmployeeInDangerOn_ReturnsDangerPayAmount()
        {
            // Arrange
            Employee employee = new Employee { IsDanger = true };

            // Act
            var actual = new SalarySlipProcessor(null).CalculateDangerPay(employee);
            var expected = Constants.DangerPayAmount;

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CalculateDangerPay_EmployeeInDangerOffAndInDangerZone_ReturnsDangerPayAmount()
        {
            // Arrange 
            var employee = new Employee { IsDanger = false, DutyStation = "Ukraine" };
            var mock = new Mock<IZoneService>();
            var setup = mock.Setup(z => z.IsDangerZone(employee.DutyStation)).Returns(true);

            // Act
            SalarySlipProcessor salarySlipProcessor = new SalarySlipProcessor(mock.Object);
            var actual = salarySlipProcessor.CalculateDangerPay(employee);
            var expected = Constants.DangerPayAmount;

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CalculateDangerPay_EmployeeInDangerOffAndNotInDangerZone_ReturnsZero()
        {
            // Arrnage
            var employee = new Employee { IsDanger = false, DutyStation = "Suadi Arabia" };
            var mock = new Mock<IZoneService>();
            var setup = mock.Setup(z => z.IsDangerZone(employee.DutyStation)).Returns(false);

            // Act
            SalarySlipProcessor salarySlipProcessor = new SalarySlipProcessor(mock.Object);
            var actual = salarySlipProcessor.CalculateDangerPay(employee);
            var expected = 0m;

            // Assert
            Assert.Equal(expected, actual);
        }
    }
}