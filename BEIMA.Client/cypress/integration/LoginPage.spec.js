/// <reference types="cypress" />

describe('Login Page', () => {
  it('Visits the Login Page', () => {
    cy.visit('http://localhost:3000/devices')
    cy.get('.sharedNavBar').contains("Logout").click();

    cy.get('[id=username]').should('exist').should('be.enabled')
    cy.get('[id=password]').should('exist').should('be.enabled')
    cy.get('[id=rememberMe]').should('exist').should('be.enabled')
    cy.get('[id=submitBtn]').should('exist').should('be.enabled')

    cy.get('.invalid-feedback').its('length').should('eq',2)
  })

  it('No errors when valid', () =>{
    cy.visit('http://localhost:3000')
    cy.get('[id=username]').scrollIntoView().clear().type("aaaaaaaa")
    cy.get('[id=password]').scrollIntoView().clear().type("aaaaaaaa")
    cy.get('[id=submitBtn]').click()

    cy.get('.invalid-feedback').its('length').should('eq',2)
    cy.get('.invalid-feedback').each((val) => {
      cy.wrap(val).should('not.be.visible')
    })
  })

  it('Input should be editable', () => {
    cy.visit('http://localhost:3000')
    cy.get('[id=username]').scrollIntoView().clear().type("aaaaaaaa")
    cy.get('[id=password]').scrollIntoView().clear().type("aaaaaaaa")
    cy.get('[id=rememberMe]').check()

    cy.get('[id=username]').should('have.value', "aaaaaaaa")
    cy.get('[id=password]').should('have.value', "aaaaaaaa")
    cy.get('[id=rememberMe]').should('be.checked')
  })
})
// The after command in support/index.js will fail since it tries to logout when the application is already logged out