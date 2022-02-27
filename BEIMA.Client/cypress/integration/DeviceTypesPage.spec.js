/// <reference types="cypress" />

describe('Device Types Page', () => {
  it('Visits the Device Types Page', () => {
      cy.visit('http://localhost:3000/deviceTypes')
      cy.get('[id=deviceTypesContent]').should('exist')
      cy.get('[id=itemList]').should('exist')
  })
})