using Capstone6.Controllers;
using Capstone6.Data;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xUnitTest
{
    public class MyUnitTestFixtures : IDisposable
    {
        public ILogger<DepartmentsAPIController> _mockLogger;
        public ApplicationDbContext _mockDbContext;
        public DepartmentsAPIController _controller;

        public MyUnitTestFixtures()
        {
            // 1. Arrange

            // Define the "Fixtures" needed to perform each of the Test.
            // - Fixtures are commonly shared objects for each of the test

            // Create an instance of the mock logger using MOQ
            _mockLogger = Mock.Of<ILogger<DepartmentsAPIController>>();

            _mockDbContext = DbContextMocker.GetApplicationDbContext("InMemoryUnitTest");

            // Create an instance of the controller 
            // the context: and logger: is to declare the input refers to this respective type
            _controller = new DepartmentsAPIController(context:_mockDbContext, logger:_mockLogger);
        }



        #region IDisposable - to dispose objects after test finish
        public void Dispose()
        {
            _mockDbContext.Dispose();
        }
        #endregion
    }
}
