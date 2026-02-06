describe("Capstone 6 Testing", function () {
  it("Testing for Capstone 6 Login", function () {
    // 1. Arrange
    let url = "https://localhost:44302/";

    // 2. Act
    // cy refers to cypress engine
    cy.visit(url);
    cy.wait(2000); //wait for 2 seconds

    cy.get("title").contains("Contoso Page - Capstone6");

    cy.get('a[name="login"]').click(); // type in the input and press the enter key
    cy.wait(2000); //wait for 2 seconds

    cy.get("title").contains("Log in - Capstone6");
    cy.get('input[name="Input.Email"]').type("demo@demo.com");
    cy.get('input[name="Input.Password"]').type("P@ssw0rd");

    cy.get('button[id="login-submit"]').click();
  });
});
