/// <reference types="cypress" />

describe('Device Type Page', () => {
  it('Visits a Device Type Page', () => {
    // visist
    cy.visit('http://localhost:3000/deviceTypes/5')

    // exist
    cy.get('[id=deviceTypeContent]').should('exist')
    cy.get('[id=itemCard]').should('exist')

    // wait
    cy.wait(1250)

    // not exist
    cy.get('[id=savebtn]').should('not.exist')
    cy.get('[id=cancelbtn]').should('not.exist')

    // exist
    cy.get('[id=editbtn]').should('exist')
    cy.get('[id=description]').should('exist')
    cy.get('[id=notes]').should('exist')
    cy.get('[id=manId]').should('exist')
    cy.get('[id=modId]').should('exist')
    cy.get('[id=serId]').should('exist')
    cy.get('[id=yearId]').should('exist')
    cy.get('[id=additionalfields]').should('exist')

    // disabled
    cy.get('[id=description]').should('be.disabled')
    cy.get('[id=notes]').should('be.disabled')

  })
  it('Enables inputs on Edit Button Click', () => {
    // visit
    cy.visit('http://localhost:3000/deviceTypes/5')

    // wait
    cy.wait(1250)

    // click
    cy.get('[id=editbtn]').click()

    // not exist
    cy.get('[id=editbtn]').should('not.exist')

    // exist
    cy.get('[id=savebtn]').should('exist')
    cy.get('[id=cancelbtn]').should('exist')
    cy.get('[id=addbtn]').should('exist')

    // enabled
    cy.get('[id=description]').should('be.enabled')
    cy.get('[id=notes]').should('be.enabled')
  })
  it('Resets fields on Cancel Button Click', () => {
    cy.visit('http://localhost:3000/deviceTypes/5')
    cy.wait(1250)
    cy.get('[id=editbtn]').click()

    cy.get('[id=description]').scrollIntoView().clear().type("Test Desc")
    cy.get('[id=notes]').scrollIntoView().clear().type("Test Notes")

    cy.get('[id=description]').should('have.value', 'Test Desc')
    cy.get('[id=notes]').should('have.value', 'Test Notes')

    cy.get('[id=cancelbtn]').click()

    cy.get('[id=description]').should('not.have.value', 'Test Desc')
    cy.get('[id=notes]').should('not.have.value', 'Test Notes')
  })
})