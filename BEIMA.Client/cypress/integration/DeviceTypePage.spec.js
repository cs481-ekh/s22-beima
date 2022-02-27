/// <reference types="cypress" />

describe('Device Type Page', () => {
  it('Visits a Device Type Page', () => {
      cy.visit('http://localhost:3000/deviceTypes/5')
      cy.get('[id=deviceTypeContent]').should('exist')
      cy.get('[id=itemCard]').should('exist')
      cy.wait(1250)
      cy.get('[id=editbtn]').should('exist')
      cy.get('[id=description]').should('exist')
      cy.get('[id=notes]').should('exist')

      cy.get('[id=manId]').should('exist')
      cy.get('[id=modId]').should('exist')
      cy.get('[id=serId]').should('exist')
      cy.get('[id=yearId]').should('exist')
      cy.get('[id=additionalfields]').should('exist')
  })
})