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
    cy.get('#Building').type("Student Union Building")
    cy.get('#Latitude').type("10.3445.2345")
    cy.get("[id='Serial Number']").type("12345")
  })
})

describe("Verify Data in fields is cleared when Add Device is selected", () => {
  it('Enter data, click Add Device, verify fields are empty', () => {
    cy.visit('http://localhost:3000/addDevice')
    cy.get('#Building').type("Student Union Building")
    cy.get('#Latitude').type("10.3445.2345")
    cy.get("[id='Serial Number']").type("12345")
    cy.get("#addDevice").scrollIntoView().click()
    cy.get('#Building').should('have.value', '')
    cy.get('#Latitude').should('have.value', '')
    cy.get("[id='Serial Number']").should('have.value', '')
  })
})