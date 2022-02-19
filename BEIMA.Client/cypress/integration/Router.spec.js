/// <reference types="cypress" />

describe("Router Doesn't Redirect On Valid Pages", () =>{
  it('Visits Root', () =>{
    cy.visit('http://localhost:3000')
    cy.url().should('eq','http://localhost:3000/')
    cy.contains('Devices')
    cy.contains('Device Types')
    cy.contains('Add Device')
    cy.contains('Add Device Type')
    cy.contains('Help')
  })
  it('Visist Help Page', () => {
    cy.visit('http://localhost:3000/help')
    cy.url().should('include', '/help')
    cy.contains('Devices')
    cy.contains('Device Types')
    cy.contains('Add Device')
    cy.contains('Add Device Type')
    cy.contains('Help')
  })
})

describe("Router Redirects On Invalid Pages", () => {
  it('Visits Invalid Page', () => {
    cy.visit('http://localhost:3000/h3lp')
    cy.url().should('eq','http://localhost:3000/')
    cy.contains('Devices')
    cy.contains('Device Types')
    cy.contains('Add Device')
    cy.contains('Add Device Type')
    cy.contains('Help')
  })
  it('Visits /', () => {
    cy.visit('http://localhost:3000/')
    cy.url().should('eq','http://localhost:3000/')
    cy.contains('Devices')
    cy.contains('Device Types')
    cy.contains('Add Device')
    cy.contains('Add Device Type')
    cy.contains('Help')
  })
})

describe("NavBar links route correctly", () => {
  it('Visits Help Page', () => {
    cy.visit('http://localhost:3000')
    cy.contains("Help").click();
    cy.url().should('include', '/Help')
    cy.contains('Devices')
    cy.contains('Device Types')
    cy.contains('Add Device')
    cy.contains('Add Device Type')
    cy.contains('Help')
  })
  /** Add more tests to click to other pages once they are created */
})