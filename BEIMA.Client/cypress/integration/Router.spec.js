/// <reference types="cypress" />
import { skipOn } from '@cypress/skip-test';

describe("Router Doesn't Redirect On Valid Pages", () =>{
  it('Visit Help Page', () => {
    skipOn('linux')
    cy.visit('http://localhost:3000/help')
    cy.url().should('include', '/help')
    cy.get('.sharedNavBar').contains('Devices')
    cy.get('.sharedNavBar').contains('Device Types')
    cy.get('.sharedNavBar').contains('Add Device')
    cy.get('.sharedNavBar').contains('Add Device Type')
    cy.get('.sharedNavBar').contains('Help')
  })
  it('Visit Device Types Page', () => {
    skipOn('linux')
    cy.visit('http://localhost:3000/deviceTypes')
    cy.url().should('include', 'deviceTypes')
    cy.get('.pageTitle').contains('Device Types')
  })
  it('Visit Devices Page', () => {
    skipOn('linux')
    cy.visit('http://localhost:3000/devices')
    cy.url().should('include', 'devices')
    cy.get('.pageTitle').contains('Devices')
  })
  it('Visit Device Page', () => {
    skipOn('linux')
    cy.visit('http://localhost:3000/devices/5')
    cy.url().should('include', 'devices/5')
    cy.get('.pageTitle').contains('View Device')
  })
  it('Visit Device Template Page', () => {
    skipOn('linux')
    cy.visit('http://localhost:3000/deviceTypes/5')
    cy.url().should('include', 'deviceTypes/5')
    cy.get('.pageTitle').contains('View Device Type')
  })
  it('Visit Login Page', () => {
    skipOn('linux')
    cy.visit('http://localhost:3000/login')
    cy.url().should('include', 'login')
    cy.get('.pageTitle').contains('Login')
  })
})

describe("Router Redirects On Invalid Pages", () => {
  it('Visits Invalid Page', () => {
    skipOn('linux')
    cy.visit('http://localhost:3000/h3lp')
    cy.url().should('eq','http://localhost:3000/devices')
    cy.get('.sharedNavBar').then(($nav) => {
      cy.wrap($nav).contains('Devices')
      cy.wrap($nav).contains('Device Types')
      cy.wrap($nav).contains('Add Device')
      cy.wrap($nav).contains('Add Device Type')
      cy.wrap($nav).contains('Buildings')
      cy.wrap($nav).contains('Help')
    })
  })
  it('Visits /', () => {
    skipOn('linux')
    cy.visit('http://localhost:3000/')
    cy.url().should('eq','http://localhost:3000/devices')
    cy.get('.sharedNavBar').then(($nav) => {
      cy.wrap($nav).contains('Devices')
      cy.wrap($nav).contains('Device Types')
      cy.wrap($nav).contains('Add Device')
      cy.wrap($nav).contains('Add Device Type')
      cy.wrap($nav).contains('Buildings')
      cy.wrap($nav).contains('Help')
    })
  })
})

describe("NavBar links route correctly", () => {
  it('Visits Help Page', () => {
    skipOn('linux')
    cy.visit('http://localhost:3000')
    cy.get('.sharedNavBar').contains("Help").click();
    cy.url().should('include', '/help')
    cy.get('.sharedNavBar').then(($nav) => {
      cy.wrap($nav).contains('Devices')
      cy.wrap($nav).contains('Device Types')
      cy.wrap($nav).contains('Add Device')
      cy.wrap($nav).contains('Add Device Type')
      cy.wrap($nav).contains('Buildings')
      cy.wrap($nav).contains('Help')
    })
  })

  it('Visit Add Device Page', () => {
    skipOn('linux')
    cy.visit('http://localhost:3000')
    cy.get('.sharedNavBar').contains("Add Device").click();
    cy.url().should('include', '/addDevice')
    cy.get('.sharedNavBar').then(($nav) => {
      cy.wrap($nav).contains('Devices')
      cy.wrap($nav).contains('Device Types')
      cy.wrap($nav).contains('Add Device')
      cy.wrap($nav).contains('Add Device Type')
      cy.wrap($nav).contains('Buildings')
      cy.wrap($nav).contains('Help')
    })
  })

  it('Visit Add Device Page', () => {
    skipOn('linux')
    cy.visit('http://localhost:3000')
    cy.get('.sharedNavBar').contains("Add Device Type").click();
    cy.url().should('include', '/addDeviceType')
    cy.get('.sharedNavBar').then(($nav) => {
      cy.wrap($nav).contains('Devices')
      cy.wrap($nav).contains('Device Types')
      cy.wrap($nav).contains('Add Device')
      cy.wrap($nav).contains('Add Device Type')
      cy.wrap($nav).contains('Buildings')
      cy.wrap($nav).contains('Help')
    })
  })

  it('Visit Buildings Page', () => {
    skipOn('linux')
    cy.visit('http://localhost:3000')
    cy.get('.sharedNavBar').contains("Buildings").click();
    cy.url().should('include', '/buildings')
    cy.get('.sharedNavBar').then(($nav) => {
      cy.wrap($nav).contains('Devices')
      cy.wrap($nav).contains('Device Types')
      cy.wrap($nav).contains('Add Device')
      cy.wrap($nav).contains('Add Device Type')
      cy.wrap($nav).contains('Buildings')
      cy.wrap($nav).contains('Help')
    })
  })
})
