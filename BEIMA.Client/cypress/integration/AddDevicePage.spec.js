/// <reference types="cypress" />

describe("Verify Buttons on Add Device Page", () => {
  it('Check for Add Device Button', () => {
    cy.visit('http://localhost:3000/addDevice')
    cy.get("#addDevice").contains('Add Device')
  })

  it('Check for Device Type DropDown', () => {
    cy.visit('http://localhost:3000/addDevice')
    cy.get("#typeDropDown").contains('Select Device Type')
  })
})

describe("Verify Data can be entered into fields", () => {
  it('Enter data into Building, Latitude, and Serial Number fields', () => {
    cy.visit('http://localhost:3000/addDevice')
    cy.get('#inputManufacturer').scrollIntoView().type("Home Depot")
    cy.get('#inputLatitude').scrollIntoView().type("10.34452345")
    cy.get("[id='inputSerial Number']").scrollIntoView().type("12345")
  })
})

describe("Verify Data in fields is cleared when Add Device is selected", () => {
  it('Enter data, click Add Device, verify fields are empty', () => {
    cy.visit('http://localhost:3000/addDevice')
    cy.get("#typeDropDown").scrollIntoView().click()
    cy.get("#typeDropDown").children().last().scrollIntoView().click()
    cy.get('#inputManufacturer').scrollIntoView().type("Home Depot")
    cy.get('#inputLatitude').scrollIntoView().type("10.34452345")
    cy.get("[id='inputSerial Number']").scrollIntoView().type("12345")
    cy.get("#addDevice").scrollIntoView().click()
    cy.contains('Proceed').click()
    cy.get('#inputManufacturer').should('have.value', '')
    cy.get('#inputLatitude').should('have.value', '')
    cy.get("[id='inputSerial Number']").should('have.value', '')
  })
})

describe("Verify Data in fields is still present when invalid coords exist and Add Device is selected", () => {
  it('Enter data, click Add Device, verify fields are empty', () => {
    cy.visit('http://localhost:3000/addDevice')
    cy.get("#typeDropDown").scrollIntoView().click()
    cy.get("#typeDropDown").children().last().scrollIntoView().click()
    cy.get('#inputManufacturer').scrollIntoView().type("Home Depot")
    cy.get('#inputLatitude').scrollIntoView().type("200")
    cy.get('#inputLongitude').scrollIntoView().type("200")
    cy.get("[id='inputSerial Number']").scrollIntoView().type("12345")
    cy.get("#addDevice").scrollIntoView().click()
    cy.get('#inputManufacturer').should('have.value', 'Home Depot')
    cy.get('#inputLatitude').should('have.value', '200')
    cy.get('#inputLongitude').should('have.value', '200')
    cy.get("[id='inputSerial Number']").should('have.value', '12345')
  })
})

describe("Verify Data in fields is still present when invalid year manufactured exist and Add Device is selected", () => {
  it('Enter data, click Add Device, verify fields are empty', () => {
    cy.visit('http://localhost:3000/addDevice')
    cy.get("#typeDropDown").scrollIntoView().click()
    cy.get("#typeDropDown").children().last().scrollIntoView().click()
    cy.get('#inputManufacturer').scrollIntoView().type("Home Depot")
    cy.get('#inputLatitude').scrollIntoView().type("34")
    cy.get('#inputLongitude').scrollIntoView().type("56")
    cy.get("[id='inputYear Manufactured']").scrollIntoView().type("Not valid")
    cy.get("#addDevice").scrollIntoView().click()
    cy.get('#inputManufacturer').should('have.value', 'Home Depot')
    cy.get('#inputLatitude').should('have.value', '34')
    cy.get('#inputLongitude').should('have.value', '56')
    cy.get("[id='inputYear Manufactured']").should('have.value', 'Not ')
  })
})

describe("Verify the dropdown has options present", function () {
  it('Count children of dropdown (see comments)', function (){
    cy.visit('http://localhost:3000/addDevice')
    cy.get("#typeDropDown").scrollIntoView().click()
    
    //get all children of the DD
    //the surrounding element for the options counts as 1
    //if there's greater than 1 then the surrounding element
    //has it's own children, or options from the DB
    cy.get("#typeDropDown").children().its('length').should('be.gt', 1)
  })
})

describe("check that selection was made", function () {
  it('Click a device and check for name change', function (){
    cy.visit('http://localhost:3000/addDevice')
    cy.get("#typeDropDown").scrollIntoView().click()
    cy.get("#typeDropDown").children().last().scrollIntoView().click()
    cy.get("#dropdown-basic").should('not.have.text', 'Select Device Type')
  })
})

describe("Verify the max character length of 1024", function () {
  it('Insert more than 1024 chars into input field, verify only 1024 are there', function (){
    cy.visit('http://localhost:3000/addDevice')
    cy.get('#inputNotes').scrollIntoView().type(randomString1024())
    cy.get('#inputNotes').should('not.include.value', 'This text should not be included')
    
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