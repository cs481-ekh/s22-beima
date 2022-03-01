/// <reference types="cypress" />

describe('Devices Page', () => {
  it('Visits the Devices Page', () => {
      cy.visit('http://localhost:3000/devices')
      cy.get('[id=devicesPageContent]').should('exist')
      cy.get('[id=itemList]').should('exist')
  })
})