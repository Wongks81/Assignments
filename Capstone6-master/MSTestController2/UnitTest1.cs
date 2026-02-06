using Microsoft.VisualStudio.TestTools.UnitTesting;
using Capstone6.Models;
using Capstone6.Controllers;
using Moq;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace MSTestController2
{
    [TestClass]
    public class UnitTest1
    {
       
        [TestMethod]
        public async Task GetIndexPage()
        {
            // 1. Arrange
            var logger = Mock.Of<ILogger<TestDepartmentsController>>();
            using var dbContext = DbContextMocker.GetApplicationDbContext("MyInMemoryUnitTest");
            var controller = new TestDepartmentsController(logger: logger, context: dbContext);

            // 2. Act
            var actionResult = await controller.Index() as ViewResult;

            // 3. Assert
            // Check if the return view is named as Index
            Assert.AreEqual("Index", actionResult.ViewName, "Result ViewName is not Index!");
            System.Console.WriteLine("Index Page View Name Check --- Pass");

            // Check if the return object is null
            Assert.IsNotNull(actionResult, "Data from Index is NULL!!");
            System.Console.WriteLine("Index Page Null Check --- Pass");

        }

        [TestMethod]
        public async Task GetSingleDepartment()
        {
            int indexToGet = 2;
            string departmentNameToGet = "Science";

            // 1. Arrange
            var logger = Mock.Of<ILogger<TestDepartmentsController>>();
            using var dbContext = DbContextMocker.GetApplicationDbContext("MyInMemoryUnitTest");
            var controller = new TestDepartmentsController(logger: logger, context: dbContext);

            // 2. Act
            var actionResult = await controller.Details(indexToGet) as ViewResult;
            
           
            // 3. Assert
            // Check if return view is named as Details
            Assert.AreEqual("Details", actionResult.ViewName, "View Page Name is not Details!");
            System.Console.WriteLine("Details Page View Name Check --- Pass");

            // Check if model returned is null
            Department actionModel = (Department)actionResult.Model;
            Assert.IsNotNull(actionModel, "Model is NULL!!");
            System.Console.WriteLine("Model Null Check --- Pass");

            // Check if return value belongs to the indexToGet record
            Assert.AreEqual(actionModel.DepartmentID, indexToGet, "Department ID is Different!");
            Assert.AreEqual(actionModel.DepartmentName, departmentNameToGet, "Department Name is Different");
            System.Console.WriteLine($"Data Record ID{indexToGet} values Check --- Pass");

        }

        [TestMethod]
        public async Task PostNewDepartment() 
        {
            // 1. Arrange
            var logger = Mock.Of<ILogger<TestDepartmentsController>>();
            using var dbContext = DbContextMocker.GetApplicationDbContext("MyInMemoryUnitTest");
            var controller = new TestDepartmentsController(logger: logger, context: dbContext);

            // test data to be added 
            int seededRecords = 3;
            var dept = new Department
            {
                DepartmentID = seededRecords +1,
                DepartmentName = "Arts"
            };

            // 2. Act - Insert the new record and return to Index page when completed
            var actionCreate = await controller.Create(dept) as RedirectToActionResult;

            // When the insert is successfully completed it will return to Index instead of Create View
            Assert.AreEqual("Index", actionCreate.ActionName, "Page View name is not Index!");
            System.Console.WriteLine("Return to Index Page when Successfull Check --- Pass");

            // Get the list of records
            var actionResult = await controller.Index() as ViewResult;
            int numOfDept = (actionResult.ViewData.Model as IList<Department>).Count;

            // 3. Assert
            // Check if the total records is 4, (Seeded 3 + added 1 )
            Assert.AreEqual(seededRecords+1, numOfDept, $"Total number of records is not {seededRecords +1} ");
            System.Console.WriteLine("Total Number of records Check --- Pass");

            foreach(Department d in (actionResult.ViewData.Model as IList<Department>))
            {
                System.Console.WriteLine($"Department ID:{d.DepartmentID} || Name: {d.DepartmentName}");
            }
        }

        [TestMethod]
        public async Task PostEditDepartment()
        {
            int indexToEdit = 2;

            // 1. Arrange
            var logger = Mock.Of<ILogger<TestDepartmentsController>>();
            using var dbContext = DbContextMocker.GetApplicationDbContext("MyInMemoryUnitTest");
            var controller = new TestDepartmentsController(logger: logger, context: dbContext);

            // 2. Act
            // Check if view name is Edit
            var actionResult = await controller.Details(indexToEdit) as ViewResult;
            
            // Check if the return model is null
            var actionModel = (Department)actionResult.Model;
            Assert.IsNotNull(actionModel, "Returned data is NULL!!");
            System.Console.WriteLine("ViewResult Model Not Null Check --- Pass");

            // Update the department name to a new name
            actionModel.DepartmentName = "Updated Name";

            // Execute the Edit function on success will return to Index View
            var actionEdit = await controller.Edit(actionModel.DepartmentID,actionModel) as RedirectToActionResult;
            Assert.AreEqual("Index", actionEdit.ActionName,"View Name is not Index!!");
            System.Console.WriteLine("Redirect to Index after success Check --- Pass");

            // 3. Assert
            // Get the record again using details function
            actionResult = await controller.Details(indexToEdit) as ViewResult;
            actionModel = (Department)actionResult.Model;

            // Check if the name is the updated name
            Assert.AreEqual("Updated Name", actionModel.DepartmentName, 
                "Edited Record Department Name is Not \" Updated Name \" !!");

            System.Console.WriteLine("Values after update Check --- Pass");

            
            System.Console.WriteLine($"Department ID:{actionModel.DepartmentID} || Name: {actionModel.DepartmentName}");
            
        }

        [TestMethod]
        public async Task PostDeleteDepartment()
        {
            int indexToDelete = 2;

            // 1. Arrange
            var logger = Mock.Of<ILogger<TestDepartmentsController>>();
            using var dbContext = DbContextMocker.GetApplicationDbContext("MyInMemoryUnitTest");
            var controller = new TestDepartmentsController(logger: logger, context: dbContext);

            // 2. Act
            var actionDelete = await controller.DeleteConfirmed(indexToDelete) as RedirectToActionResult;

            // Check if the View Name is Index, after success will redirect to index
            Assert.AreEqual("Index", actionDelete.ActionName, "View Name is not Index!");
            System.Console.WriteLine("Return to Index after delete --- Pass");

            // Check if the Record is still in the database
            // Try to get the object values using the ID
            var actionResult = await controller.Details(indexToDelete) as ViewResult;

            // Check if Null is returned, if returned null means the delete is successful
            Assert.IsNull(actionResult, "Record still Exist as it is not NULL!");
            System.Console.WriteLine("Null Check for successful deletion --- Pass");

        }

        [TestMethod]
        public async Task DepartmentsModelValidation()
        {
            #region testdata
            // test data to be added
            // Validation for the data annotations on String Length will only work when connected to DB
            // InMemory does not check this.
            //-------------------------------------------------------------------------
               var dept = new Department
               {
                   DepartmentID = 4,
                   DepartmentName = "asdaddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddBSdddddddddddddddddddddddddddfffffffffffffgggggggggghjhjkkll;;"
               };
            #endregion

            // 1. Arrange
            var logger = Mock.Of<ILogger<TestDepartmentsController>>();
            using var dbContext = DbContextMocker.GetApplicationDbContext("MyInMemoryUnitTest");
            var controller = new TestDepartmentsController(logger: logger, context: dbContext);

            // 2. Act
            controller.ModelState.AddModelError("Error from Unit Test", "Error from Test Method");

            var actionResult = await controller.Create(dept) as ViewResult;
            Assert.AreEqual(actionResult.ViewName, "Create", "View Name is not Create!");
            System.Console.WriteLine("View Name remain at Create --- Pass");

            var modelState = actionResult.ViewData.ModelState;
            Assert.AreEqual<bool>(false, modelState.IsValid,
                "Something is wrong, injected error not detected.", "Model is valid! Did someone clear all errors??");
            System.Console.WriteLine("Invalid Model State --- Pass");

            Assert.IsTrue(modelState.ContainsKey("Error from Unit Test"));
            Assert.AreEqual<string>(
                "Error from Test Method",
                modelState["Error from Unit Test"].Errors[0].ErrorMessage,
                "ModelState Error is not the injected Error! Please check!");
            System.Console.WriteLine("Check Error Message --- Pass");
        }

        [TestMethod]
        public async Task InsertWrongDataValidation()
        {
            int idToTest = 422;
            

            var dept = new Department
            {
                DepartmentID = 4,
                DepartmentName = null
            };

            // 1. Arrange
            var logger = Mock.Of<ILogger<TestDepartmentsController>>();
            using var dbContext = DbContextMocker.GetApplicationDbContext("MyInMemoryUnitTest");
            var controller = new TestDepartmentsController(logger: logger, context: dbContext);

            // Try to find a record which does not exist
            var actionResult = await controller.Details(idToTest);
            Assert.IsInstanceOfType(actionResult, typeof(NotFoundResult), "Error data being accepted please check!");
            System.Console.WriteLine("Not Found Result returned --- Pass");

            actionResult = await controller.Create(dept);
            Assert.IsInstanceOfType(actionResult, typeof(BadRequestResult), "NULL data being accepted please check!");
            System.Console.WriteLine("Department Name null value check --- Pass");



        }
    }
}
