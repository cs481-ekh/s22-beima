/// <reference types="cypress" />

describe("Verify Buttons on Add Device Type Page", () => {
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
  it('Enter data into Name, Description, and Notes field', () => {
    cy.visit('http://localhost:3000/addDeviceType')
    cy.get('#inputName').scrollIntoView().type("new type")
    cy.get('#inputDescription').scrollIntoView().type("newly added type")
    cy.get("[id='inputDevice Type Notes']").scrollIntoView().type("meter from SEL")
  })
})

describe("Verify Data in fields is cleared when Add Device Type is selected", () => {
  it('Enter data, click Add Device, verify fields are empty', () => {
    cy.visit('http://localhost:3000/addDeviceType')
    cy.get('#inputName').scrollIntoView().type("new type")
    cy.get('#inputDescription').scrollIntoView().type("newly added type")
    cy.get("[id='inputDevice Type Notes']").scrollIntoView().type("meter from SEL")
    cy.get("#addDeviceType").scrollIntoView().click()
    cy.get('#inputName').should('have.value', '')
    cy.get('#inputDescription').should('have.value', '')
    cy.get("[id='inputDevice Type Notes']").should('have.value', '')
  })
})

describe("Verify custom fields can be added", () => {
  it('Add new fields', () => {
    cy.visit('http://localhost:3000/addDeviceType')
    cy.get('#newField').scrollIntoView().type("field1")
    cy.get("#addField").scrollIntoView().click()
    cy.get('#newField').scrollIntoView().type("field2")
    cy.get("#addField").scrollIntoView().click()
    cy.get('#newField').scrollIntoView().type("field3")
    cy.get("#addField").scrollIntoView().click()
    cy.get('#customFields').then(($custfields) => {
      cy.wrap($custfields).contains('field1')
      cy.wrap($custfields).contains('field2')
      cy.wrap($custfields).contains('field3')
    })
  })
})

describe("Verify custom fields can be deleted", () => {
  it('Add new fields', () => {
    cy.visit('http://localhost:3000/addDeviceType')
    cy.get('#newField').scrollIntoView().type("field1")
    cy.get("#addField").scrollIntoView().click('left')
    cy.get('#newField').scrollIntoView().type("field2")
    cy.get("#addField").scrollIntoView().click('left')
    cy.get('#customFields').then(($custfields) => {
      cy.wrap($custfields).contains('field1')
      cy.wrap($custfields).contains('field2')
      cy.wrap($custfields).get('#field2').get("#removefield2").scrollIntoView().click()
      cy.wrap($custfields).contains('field1')
      cy.wrap($custfields).children().should('not.contain', 'field2')
    })
  })
})

describe("Verify custom fields get cleared when Add Device Type is clicked", () => {
  it('Add new fields', () => {
    cy.visit('http://localhost:3000/addDeviceType')
    cy.get('#newField').scrollIntoView().type("field1")
    cy.get("#addField").scrollIntoView().click('left')
    cy.get('#newField').scrollIntoView().type("field2")
    cy.get("#addField").scrollIntoView().click('left')
    cy.get('#newField').scrollIntoView().type("field3")
    cy.get("#addField").scrollIntoView().click('left')
    cy.get('#customFields').then(($custfields) => {
      cy.wrap($custfields).contains('field1')
      cy.wrap($custfields).contains('field2')
      cy.wrap($custfields).contains('field3')
    })
    cy.get("#addDeviceType").scrollIntoView().click('left')
    cy.get('#customFields').then(($custfields) => {
      cy.wrap($custfields).children().should('not.contain', 'field1')
      cy.wrap($custfields).children().should('not.contain', 'field2')
      cy.wrap($custfields).children().should('not.contain', 'field3')
    })
  })
})