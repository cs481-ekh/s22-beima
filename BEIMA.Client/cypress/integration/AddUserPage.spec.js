/// <reference types="cypress" />

describe("Verify Buttons on Add User Page", () => {
  it('Check for Add User Button', () => {
    cy.visit('http://localhost:3000/users/addUser')
    cy.get("#addUser").contains('Add User')
  })

  it('Check for back button', () => {
    cy.visit('http://localhost:3000/users/addUser')
    cy.get("#backUsers")
  })
})

describe("Verify Data can be entered into fields", () => {
  it('Enter data into username, role, first name, last name fields', () => {
    cy.visit('http://localhost:3000/users/addUser')
    cy.get('#inputUsername').scrollIntoView().type("firstLast")
    cy.get("#typeDropDown").scrollIntoView().click('left')
    cy.get("#typeDropDown").children().last().scrollIntoView().click()
    cy.get("#dropdown-basic").should('not.have.text', 'Select Role')
  })
})

// make sure this user is removed from the database before this test runs, or it wil fail
describe("Verify Data in fields is cleared when Add User is selected with valid data", () => {
  it('Enter data, click Add User, verify fields are empty', () => {
    cy.visit('http://localhost:3000/users/addUser')
    cy.get('#inputUsername').scrollIntoView().type("firstLast")
    cy.get("[id='First Name']").scrollIntoView().type("first")
    cy.get("[id='Last Name']").scrollIntoView().type("Last")
    cy.get("#typeDropDown").scrollIntoView().click('left')
    cy.get("#typeDropDown").children().last().scrollIntoView().click()
    cy.get('#inputPassword').scrollIntoView().type("fvghfdghgfQ1!")
    cy.get('input[name="Password Confirmation"]').type('fvghfdghgfQ1!')
    cy.get("#addUser").scrollIntoView().click()
    cy.contains('Dismiss').click()
    cy.get('#inputUsername').should('have.value', '')
    cy.get("#dropdown-basic").should('have.text', 'Select Role')
  })
})

describe("Verify Data in fields is still present when invalid password is present", () => {
  it('Enter data, click Add User, verify fields are empty', () => {
    cy.visit('http://localhost:3000/users/addUser')
    cy.get('#inputUsername').scrollIntoView().type("firstLast")
    cy.get("#typeDropDown").scrollIntoView().click('left')
    cy.get("#typeDropDown").children().last().scrollIntoView().click()
    cy.get('#inputPassword').scrollIntoView().type("200")
    cy.get("#addUser").scrollIntoView().click()
    cy.get('#inputUsername').should('have.value', 'firstLast')
    cy.get("#dropdown-basic").should('not.have.text', 'Select Role')
    cy.get('#inputPassword').should('have.value', '200')
  })
})

describe("Verify the max character length of 1024", function () {
    it('Insert more than 1024 chars into input field, verify only 1024 are there', function (){
      cy.visit('http://localhost:3000/users/addUser')
      cy.get('#inputUsername').scrollIntoView().type(randomString1024())
      cy.get('#inputUsername').should('not.include.value', 'This text should not be included')
      
      function randomString1024() {
        var text = "";
        var possible = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
    
        for (var i = 0; i < 1024; i++){
          text += possible.charAt(Math.floor(Math.random() * possible.length));
        }
        text += "This text should not be included";
  
        return text;
      }
    })
  })