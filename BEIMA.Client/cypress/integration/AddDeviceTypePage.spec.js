/// <reference types="cypress" />

describe("Verify Buttons on Add Device Page", () => {
  it('Check for Add Device Type Button', () => {
    cy.visit('http://localhost:3000/addDeviceType')
    cy.get("#addDeviceType").contains('Add Device Type')
  })

  it('Check for Add Field Button', () => {
    cy.visit('http://localhost:3000/addDeviceType')
    cy.get("#addField").contains('Add Field')
  })
})

describe("Verify Data can be entered into fields", () => {
  it('Enter data into Name field', () => {
    cy.visit('http://localhost:3000/addDeviceType')
    cy.get('#Name').type("new type")
  })
  it('Enter data into Description field', () => {
    cy.visit('http://localhost:3000/addDeviceType')
    cy.get('#Description').type("newly added type")
  })
  it('Enter data into Device Type Notes field', () => {
    cy.visit('http://localhost:3000/addDeviceType')
    cy.get("[id='Device Type Notes']").type("meter from SEL")
  })
})

describe("Verify Data in fields is cleared when Add Device is selected", () => {
  it('Enter data, click Add Device, verify fields are empty', () => {
    cy.visit('http://localhost:3000/addDeviceType')
    cy.get('#Name').type("new type")
    cy.get('#Description').type("newly added type")
    cy.get("[id='Device Type Notes']").type("meter from SEL")
    cy.get("#addDeviceType").click()
    cy.get('#Name').should('have.value', '')
    cy.get('#Description').should('have.value', '')
    cy.get("[id='Device Type Notes']").should('have.value', '')
  })
})

describe("Verify custom fields can be added", () => {
  it('Add new fields', () => {
    cy.visit('http://localhost:3000/addDeviceType')
    cy.get('#newField').type("field1")
    cy.get("#addField").click()
    cy.get('#newField').type("field2")
    cy.get("#addField").click()
    cy.get('#newField').type("field3")
    cy.get("#addField").click()
    cy.get('#customFields').contains('field1')
    cy.get('#customFields').contains('field2')
    cy.get('#customFields').contains('field3')
  })
})

describe("Verify custom fields can be deleted", () => {
  it('Add new fields', () => {
    cy.visit('http://localhost:3000/addDeviceType')
    cy.get('#newField').type("field1")
    cy.get("#addField").click()
    cy.get('#newField').type("field2")
    cy.get("#addField").click()
    cy.get('#customFields').contains('field1')
    cy.get('#customFields').contains('field2')
    cy.get('#customFields').get('#field2').get("#removefield2").click()
    cy.get('#customFields').contains('field1')
    cy.get('#customFields').children().should('not.contain', 'field2')
  })
})

describe("Verify custom fields get cleared when Add Device Type is clicked", () => {
  it('Add new fields', () => {
    cy.visit('http://localhost:3000/addDeviceType')
    cy.get('#newField').type("field1")
    cy.get("#addField").click()
    cy.get('#newField').type("field2")
    cy.get("#addField").click()
    cy.get('#newField').type("field3")
    cy.get("#addField").click()
    cy.get('#customFields').contains('field1')
    cy.get('#customFields').contains('field2')
    cy.get('#customFields').contains('field3')
    cy.get("#addDeviceType").click()
    cy.get('#customFields').children().should('not.contain', 'field1')
    cy.get('#customFields').children().should('not.contain', 'field2')
    cy.get('#customFields').children().should('not.contain', 'field3')
  })
})