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
    cy.get("[id='inputNotes']").scrollIntoView().type("meter from SEL")
  })
})

describe("Verify Data in fields is cleared when Add Device Type is selected", () => {
  it('Enter data, click Add Device, verify fields are empty', () => {
    cy.visit('http://localhost:3000/addDeviceType')
    cy.get('#inputName').scrollIntoView().type("new type")
    cy.get('#inputDescription').scrollIntoView().type("newly added type")
    cy.get("[id='inputNotes']").scrollIntoView().type("meter from SEL")
    cy.get("#addDeviceType").scrollIntoView().click()
    cy.get('#inputName').should('have.value', '')
    cy.get('#inputDescription').should('have.value', '')
    cy.get("[id='inputNotes']").should('have.value', '')
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
    cy.get("#addField").scrollIntoView().click()
    cy.get('#newField').scrollIntoView().type("field2")
    cy.get("#addField").scrollIntoView().click()
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
    cy.get("#addDeviceType").scrollIntoView().click()
    cy.get('#customFields').then(($custfields) => {
      cy.wrap($custfields).children().should('not.contain', 'field1')
      cy.wrap($custfields).children().should('not.contain', 'field2')
      cy.wrap($custfields).children().should('not.contain', 'field3')
    })
  })
})

describe("Verify error message on invalid custom field inputs", () => {
  it('Add duplicate field, verify it only gets added once', () => {
    cy.visit('http://localhost:3000/addDeviceType')
    cy.get('#newField').scrollIntoView().type("field1")
    cy.get("#addField").scrollIntoView().click()
    cy.get('#newField').scrollIntoView().type("field1")
    cy.get("#addField").scrollIntoView().click()
    cy.get('#customFields').then(($custfields) => {
      cy.wrap($custfields).contains('field1')
      cy.wrap($custfields).get('#field1').get("#removefield1").scrollIntoView().click()
      cy.wrap($custfields).children().should('not.contain', 'field1')
    })
  })

  it('Add empty field, verify it is not', () => {
    cy.visit('http://localhost:3000/addDeviceType')
    cy.get('#newField').scrollIntoView().type("   ")
    cy.get("#addField").scrollIntoView().click()
    cy.get('#customFields').then(($custfields) => {
      cy.wrap($custfields).children().should('not.contain', '   ')
    })
  })

  it('Input no field data, click add field, verify it does not get added', () => {
    cy.visit('http://localhost:3000/addDeviceType')
    cy.get("#addField").scrollIntoView().click()
    cy.get('#customFields').then(($custfields) => {
      cy.wrap($custfields).children().should('be.empty')
    })
  })
})

describe("Verify the max character length of 1024", function () {
  it('Insert more than 1024 chars into input field, verify only 1024 are there', function (){
    cy.visit('http://localhost:3000/addDeviceType')
    cy.get('#inputName').scrollIntoView().type(randomString1024())
    cy.get('#inputName').should('not.include.value', 'This text should not be included')
    
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