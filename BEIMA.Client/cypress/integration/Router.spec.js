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
  it('Visit Device Types Page', () => {
    cy.visit('http://localhost:3000/deviceTypes')
    cy.url().should('include', 'deviceTypes')
    cy.get('.pageTitle').contains('Device Types')
  })
  it('Visit Devices Page', () => {
    cy.visit('http://localhost:3000/devices')
    cy.url().should('include', 'devices')
    cy.get('.pageTitle').contains('Devices')
  })
  it('Visit Device Page', () => {
    cy.visit('http://localhost:3000/devices/5')
    cy.url().should('include', 'devices/5')
    cy.get('.pageTitle').contains('View Device')
  })
  it('Visit Device Template Page', () => {
    cy.visit('http://localhost:3000/deviceTypes/5')
    cy.url().should('include', 'deviceTypes/5')
    cy.get('.pageTitle').contains('View Device Type')
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
  /** Add more tests to click to other pages once they are created */
})