// ***********************************************************
// This example support/index.js is processed and
// loaded automatically before your test files.
//
// This is a great place to put global configuration and
// behavior that modifies Cypress.
//
// You can change the location of this file or turn off
// automatically serving support files with the
// 'supportFile' configuration option.
//
// You can read more here:
// https://on.cypress.io/configuration
// ***********************************************************

// Import commands.js using ES2015 syntax:
import './commands'

// Alternatively you can use CommonJS syntax:
// require('./commands')
/// <reference types="cypress" />

before(() => {
  cy.visit('http://localhost:3000/')
  cy.get('[id=username]').scrollIntoView().clear().type("testuser")

  // REPLACE PASSWORD WITH VALID PASSWORD FROM TEST USER IN DATABASE
  cy.get('[id=password]').scrollIntoView().clear().type("testUser1!")
  cy.get('[id=submitBtn]').click()

  cy.wait(700)
})

after(() => {
  cy.get('.sharedNavBar').contains("Logout").click();
})
