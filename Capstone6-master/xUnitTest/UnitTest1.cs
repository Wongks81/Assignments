using Capstone6.Models;
using Capstone6.Controllers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Moq;
using Microsoft.Extensions.Logging;
using System.Net;

namespace xUnitTest
{
    public class UnitTest1 
    {
        private readonly ITestOutputHelper _testOutputHelper;
        

        public UnitTest1(ITestOutputHelper testOutputhelper)
        {
            _testOutputHelper = testOutputhelper;
        }

        [Fact]
        public async Task TestGetAllDepartments()
        {
            // test data
            int numOfRecords = 3;

            // 1. Arrange
            var logger = Mock.Of<ILogger<DepartmentsAPIController>>();
            using var dbContext = DbContextMocker.GetApplicationDbContext("MyInMemoryUnitTest");
            var controller = new DepartmentsAPIController(logger:logger, context:dbContext);

            // 2. Act
            var actionResult = await controller.GetDepartments();
            var departments = actionResult.Value;

            // 3. Assert
            // check if there is any return data
            _testOutputHelper.WriteLine(" >> Return data check");
            Assert.NotNull(departments);
            _testOutputHelper.WriteLine(" --- Return Data is not NULL ---");

            // check if the number of seeded data is equal to the retrieve data
            int numberOfDepartments = (departments as IList<Department>).Count;

            _testOutputHelper.WriteLine(" >> Returned records count check");
            Assert.Equal(numOfRecords, numberOfDepartments);
            _testOutputHelper.WriteLine(" --- Returned records count is same as seeded data ---");

            // 3. Assert
            _testOutputHelper.WriteLine(" --- Departments in the InMemory DB ---");
            foreach(Department dept in departments)
            {
                _testOutputHelper.WriteLine($"{dept.DepartmentID} {dept.DepartmentName}");
            }

            _testOutputHelper.WriteLine("Number of Departments: {0}", (departments as IList<Department>).Count);

        }

        [Fact]
        public async Task TestGetSingleDepartment()
        {
            // 1. Arrange
            var logger = Mock.Of<ILogger<DepartmentsAPIController>>();
            using var dbContext = DbContextMocker.GetApplicationDbContext("MyInMemoryUnitTest");
            var controller = new DepartmentsAPIController(logger: logger, context: dbContext);

            // 2. Act - get the 2nd department using department id
            var actionResult = await controller.GetDepartment(2);
            var department = actionResult.Value;

            // 3. Assert
            // check if there is any return data
            _testOutputHelper.WriteLine(" >> Return data check");
            Assert.NotNull(department);
            _testOutputHelper.WriteLine(" --- Return Data is not NULL ---");

            // check if the number of seeded data is equal to the retrieve data
            _testOutputHelper.WriteLine(" >> Return data Values check");
            Assert.Equal(2, department.DepartmentID);
            _testOutputHelper.WriteLine(" --- Department ID is the same ---");
            Assert.Equal("Science", department.DepartmentName);
            _testOutputHelper.WriteLine(" --- Department Name is the same ---");

        }

        [Fact]
        public async Task AddDepartment()
        {
            // 1. Arrange
            var logger = Mock.Of<ILogger<DepartmentsAPIController>>();
            using var dbContext = DbContextMocker.GetApplicationDbContext("MyInMemoryUnitTest");
            var controller = new DepartmentsAPIController(logger: logger, context: dbContext);

            // test data to be added 
            var dept = new Department
            {
                DepartmentID = 4,
                DepartmentName = "Arts"
            };

            // 2. Act - get the 2nd department using department id
            var action = await controller.PostDepartment(dept);

            // use this as we need the return department object as well as the id
            var actionResult = action.Result as CreatedAtActionResult;  //OkObjectResult


            // 3. Assert
            // check if there OKObjectResult is obtained
            _testOutputHelper.WriteLine(" >> Return result check");
            Assert.NotNull(actionResult);
            _testOutputHelper.WriteLine(" --- Action Result is OKObjectResult ---");

            // Check if the department was added successfully to the inmemory
            var deptAdded = actionResult.Value as Department;
            _testOutputHelper.WriteLine(" >> Records Check");
            Assert.NotNull(deptAdded);
            _testOutputHelper.WriteLine(" --- Record is not NULL ---");

            // ####  DO NOT compare ID as ID maybe database generated  #####
            _testOutputHelper.WriteLine(" >> New Record Department Name Check");
            Assert.Equal(dept.DepartmentName, deptAdded.DepartmentName);
            _testOutputHelper.WriteLine("-- New Record Department value is correct --");

            // EXTRA to list out the department list
            // 2. Act
            var actionResult2 = await controller.GetDepartments();
            var departments2 = actionResult2.Value;


            // 3. Assert
            Assert.NotNull(departments2);

            _testOutputHelper.WriteLine(" --- Departments in the InMemory DB ---");
            foreach (var dept2 in departments2)
            {
                _testOutputHelper.WriteLine($"{dept2.DepartmentID} {dept2.DepartmentName}");
            }

            _testOutputHelper.WriteLine("Number of Departments: {0}", (departments2 as IList<Department>).Count);

        }

        [Fact]
        public async Task UpdateDepartment()
        {
            // test data to be updated 
            var dept2 = new Department
            {
                DepartmentID = 1,
                DepartmentName = "Updated Dept Name"
            };

            // 1. Arrange
            var logger = Mock.Of<ILogger<DepartmentsAPIController>>();
            using var dbContext = DbContextMocker.GetApplicationDbContext("MyInMemoryUnitTest");
            var controller = new DepartmentsAPIController(logger: logger, context: dbContext);

            // 2. Act - Get the department object and check if the result is null
            var actionResult = await controller.GetDepartment(dept2.DepartmentID);
            _testOutputHelper.WriteLine(" >> Return records Check");
            Assert.NotNull(actionResult.Value);
            _testOutputHelper.WriteLine($"--- Data Retrieved for ID {dept2.DepartmentID} ---");

            // Update the Value of the record
            actionResult.Value.DepartmentName = dept2.DepartmentName;

            // Execute the PUT - API return NoContext when successfully
            _testOutputHelper.WriteLine(" >> Put Action Check");
            var actionPutResult = await controller.PutDepartment(dept2.DepartmentID, actionResult.Value);
            Assert.IsType<OkResult>(actionPutResult);
            _testOutputHelper.WriteLine(" --- Put Action Done ---");

            // Get the Department again to check if the value is updated
            var updatedResult = await controller.GetDepartment(dept2.DepartmentID);
            _testOutputHelper.WriteLine(" >> Updated Value Check");
            Assert.Equal(dept2.DepartmentName, updatedResult.Value.DepartmentName);
            _testOutputHelper.WriteLine($" --- Updated ID {updatedResult.Value.DepartmentID} " +
                $"with {updatedResult.Value.DepartmentName} ---"); ;


        }

        [Fact]
        public async Task DeleteDepartment()
        {
            // ID for testing to be deleted
            int deleteID = 1;

            // 1. Arrange
            var logger = Mock.Of<ILogger<DepartmentsAPIController>>();
            using var dbContext = DbContextMocker.GetApplicationDbContext("MyInMemoryUnitTest");
            var controller = new DepartmentsAPIController(logger: logger, context: dbContext);

            // 2. Execute the Delete - API return NoContext so no variable assigned
            _testOutputHelper.WriteLine(" >> Delete Action Check");
            var actionResult = await controller.DeleteDepartment(deleteID);
            _testOutputHelper.WriteLine($" --- Delete Done for ID {deleteID} ---");

            // 3. Assert check if the data is deleted
            // Try to get the object values using the ID
            var action = await controller.GetDepartment(deleteID);


            // Check if NotFoundResult is returned, if returned means the delete is successful
            _testOutputHelper.WriteLine(" >> Record exist after delete Check");
            Assert.IsType<NotFoundResult>(action.Result);
            _testOutputHelper.WriteLine(" --- Record not found, Delete SUcce");
        }

        [Fact]
        public async Task InjectWrongData()
        {
            // test data
            int idToTest = 422;

            var dept = new Department
            {
                DepartmentID = 4,
                DepartmentName = null
            };

            // 1. Arrange
            var logger = Mock.Of<ILogger<DepartmentsAPIController>>();
            using var dbContext = DbContextMocker.GetApplicationDbContext("MyInMemoryUnitTest");
            var controller = new DepartmentsAPIController(logger: logger, context: dbContext);

            // Inject a non existent ID, should have a NotFound Result
            _testOutputHelper.WriteLine(" >> Inserting wrong ID Check");
            var actionResult = await controller.GetDepartment(idToTest);
            Assert.IsType<NotFoundResult>(actionResult.Result);
            _testOutputHelper.WriteLine(" --- Not Found Result returned, Check success");

            // Inject a null Department Name should return BadRequest
            _testOutputHelper.WriteLine(" >> DepartmentName Null Check");
            actionResult = await controller.PostDepartment(dept);
            Assert.IsType<BadRequestResult>(actionResult.Result);
            _testOutputHelper.WriteLine(" --- Bad Request received, Check success");



        }

    }
}
