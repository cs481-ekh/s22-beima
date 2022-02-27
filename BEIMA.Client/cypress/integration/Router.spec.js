/// <reference types="cypress" />

describe("Router Doesn't Redirect On Valid Pages", () =>{
  it('Visits Root', () =>{
    cy.visit('http://localhost:3000')
    cy.url().should('eq','http://localhost:3000/')
    cy.get('.sharedNavBar').contains('Devices')
    cy.get('.sharedNavBar').contains('Device Types')
    cy.get('.sharedNavBar').contains('Add Device')
    cy.get('.sharedNavBar').contains('Add Device Type')
    cy.get('.sharedNavBar').contains('Help')
  })
  it('Visit Help Page', () => {
    cy.visit('http://localhost:3000/help')
    cy.url().should('include', '/help')
    cy.get('.sharedNavBar').contains('Devices')
    cy.get('.sharedNavBar').contains('Device Types')
    cy.get('.sharedNavBar').contains('Add Device')
    cy.get('.sharedNavBar').contains('Add Device Type')
    cy.get('.sharedNavBar').contains('Help')
  })
})

describe("Router Redirects On Invalid Pages", () => {
  it('Visits Invalid Page', () => {
    cy.visit('http://localhost:3000/h3lp')
    cy.url().should('eq','http://localhost:3000/')
    cy.get('.sharedNavBar').contains('Devices')
    cy.get('.sharedNavBar').contains('Device Types')
    cy.get('.sharedNavBar').contains('Add Device')
    cy.get('.sharedNavBar').contains('Add Device Type')
    cy.get('.sharedNavBar').contains('Help')
  })
  it('Visits /', () => {
    cy.visit('http://localhost:3000/')
    cy.url().should('eq','http://localhost:3000/')
    cy.get('.sharedNavBar').contains('Devices')
    cy.get('.sharedNavBar').contains('Device Types')
    cy.get('.sharedNavBar').contains('Add Device')
    cy.get('.sharedNavBar').contains('Add Device Type')
    cy.get('.sharedNavBar').contains('Help')
  })
})

describe("NavBar links route correctly", () => {
  it('Visits Help Page', () => {
    cy.visit('http://localhost:3000')
    cy.get('.sharedNavBar').contains("Help").click();
    cy.url().should('include', '/help')
    cy.get('.sharedNavBar').contains('Devices')
    cy.get('.sharedNavBar').contains('Device Types')
    cy.get('.sharedNavBar').contains('Add Device')
    cy.get('.sharedNavBar').contains('Add Device Type')
    cy.get('.sharedNavBar').contains('Help')
  })

  it('Visit Add Device Page', () => {
    cy.visit('http://localhost:3000')
    cy.get('.sharedNavBar').contains("Add Device").click();
    cy.url().should('include', '/addDevice')
    cy.get('.sharedNavBar').contains('Devices')
    cy.get('.sharedNavBar').contains('Device Types')
    cy.get('.sharedNavBar').contains('Add Device')
    cy.get('.sharedNavBar').contains('Add Device Type')
    cy.get('.sharedNavBar').contains('Help')
  })

  it('Visit Add Device Page', () => {
    cy.visit('http://localhost:3000')
    cy.get('.sharedNavBar').contains("Add Device Type").click();
    cy.url().should('include', '/addDeviceType')
    cy.get('.sharedNavBar').contains('Devices')
    cy.get('.sharedNavBar').contains('Device Types')
    cy.get('.sharedNavBar').contains('Add Device')
    cy.get('.sharedNavBar').contains('Add Device Type')
    cy.get('.sharedNavBar').contains('Help')
  })
})
