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
    cy.get("#inputRole").scrollIntoView().type("admin")
  })
})

describe("Verify Data in fields is cleared when Add User is selected", () => {
  it('Enter data, click Add User, verify fields are empty', () => {
    cy.visit('http://localhost:3000/users/addUser')
    cy.get('#inputUsername').scrollIntoView().type("firstLast")
    cy.get("#inputRole").scrollIntoView().type("admin")
    cy.get("#addUser").scrollIntoView().click()
    cy.get('#inputUsername').should('have.value', '')
    cy.get("#inputRole").should('have.value', '')
  })
})

describe("Verify the max character length of 1024", function () {
    it('Insert more than 1024 chars into input field, verify only 1024 are there', function (){
      cy.visit('http://localhost:3000/users/addUser')
      cy.get('#inputRole').scrollIntoView().type(randomString1024())
      cy.get('#inputRole').should('not.include.value', 'This text should not be included')
      
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