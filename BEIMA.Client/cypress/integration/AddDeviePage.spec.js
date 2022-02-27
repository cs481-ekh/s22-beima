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
    cy.get('#inputBuilding').scrollIntoView().type("Student Union Building")
    cy.get('#inputLatitude').scrollIntoView().type("10.3445.2345")
    cy.get("[id='inputSerial Number']").scrollIntoView().type("12345")
  })
})

describe("Verify Data in fields is cleared when Add Device is selected", () => {
  it('Enter data, click Add Device, verify fields are empty', () => {
    cy.visit('http://localhost:3000/addDevice')
    cy.get('#inputBuilding').scrollIntoView().type("Student Union Building")
    cy.get('#inputLatitude').scrollIntoView().type("10.3445.2345")
    cy.get("[id='inputSerial Number']").scrollIntoView().type("12345")
    cy.get("#addDevice").scrollIntoView().click()
    cy.get('#inputBuilding').should('have.value', '')
    cy.get('#inputLatitude').should('have.value', '')
    cy.get("[id='inputSerial Number']").should('have.value', '')
  })
})