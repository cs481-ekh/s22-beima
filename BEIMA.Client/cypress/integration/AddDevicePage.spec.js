/// <reference types="cypress" />
import { skipOn } from '@cypress/skip-test';


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
    skipOn('linux')
    cy.visit('http://localhost:3000/addDevice')
    cy.get('#inputBuilding').scrollIntoView().type("Student Union Building")
    cy.get('#inputLatitude').scrollIntoView().type("10.34452345")
    cy.get("[id='inputSerial Number']").scrollIntoView().type("12345")
  })
})

describe("Verify Data in fields is cleared when Add Device is selected", () => {
  it('Enter data, click Add Device, verify fields are empty', () => {
    skipOn('linux')
    cy.visit('http://localhost:3000/addDevice')
    cy.get('#inputBuilding').scrollIntoView().type("Student Union Building")
    cy.get('#inputLatitude').scrollIntoView().type("10.34452345")
    cy.get("[id='inputSerial Number']").scrollIntoView().type("12345")
    cy.get("#addDevice").scrollIntoView().click()
    cy.get('#inputBuilding').should('have.value', '')
    cy.get('#inputLatitude').should('have.value', '')
    cy.get("[id='inputSerial Number']").should('have.value', '')
  })
})

describe("Verify Data in fields is still present when invalid coords exist and Add Device is selected", () => {
  it('Enter data, click Add Device, verify fields are empty', () => {
    skipOn('linux')
    cy.visit('http://localhost:3000/addDevice')
    cy.get('#inputBuilding').scrollIntoView().type("Student Union Building")
    cy.get('#inputLatitude').scrollIntoView().type("200")
    cy.get('#inputLongitude').scrollIntoView().type("200")
    cy.get("[id='inputSerial Number']").scrollIntoView().type("12345")
    cy.get("#addDevice").scrollIntoView().click()
    cy.get('#inputBuilding').should('have.value', 'Student Union Building')
    cy.get('#inputLatitude').should('have.value', '200')
    cy.get('#inputLongitude').should('have.value', '200')
    cy.get("[id='inputSerial Number']").should('have.value', '12345')
  })
})

describe("Verify the dropdown has options present", function () {
  it('Count children of dropdown (see comments)', function (){
    skipOn('linux')
    
    cy.visit('http://localhost:3000/addDevice')
    cy.get("#typeDropDown").scrollIntoView().click()
    
    //get all children of the DD
    //the surrounding element for the options counts as 1
    //if there's greater than 1 then the surrounding element
    //has it's own children, or options from the DB
    cy.get("#typeDropDown").children().its('length').should('be.gt', 1)
  })
})

