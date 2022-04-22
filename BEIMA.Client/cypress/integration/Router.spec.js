/// <reference types="cypress" />

describe("Router Doesn't Redirect On Valid Pages", () =>{
  // WHEN TESTING, CHANGE THIS TO A VALID DEVICE TYPE ID THAT IS IN THE DATABASE
  let deviceTypeID = ''

   // WHEN TESTING, CHANGE THIS TO A VALID DEVICE ID THAT IS IN THE DATABASE
   let deviceID = ''

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
  })
  it('Visit Devices Page', () => {
    cy.visit('http://localhost:3000/devices')
    cy.url().should('include', 'devices')
  })
  it('Visit Device Page', () => {
    cy.visit('http://localhost:3000/devices/' + deviceID)
    cy.url().should('include', 'devices/' + deviceID)
  })
  it('Visit Device Template Page', () => {
    cy.visit('http://localhost:3000/deviceTypes/' + deviceTypeID)
    cy.url().should('include', 'deviceTypes/' + deviceTypeID)
  })
  // We do not test for routing to login page
  // since it is only accessible when logged out
})

describe("Router Redirects On Invalid Pages", () => {
  it('Visits Invalid Page', () => {
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
  
  it('Visit Users Page', () => {
    cy.visit('http://localhost:3000')
    cy.get('.sharedNavBar').contains("Users").click();
    cy.url().should('include', '/users')
    cy.get('.sharedNavBar').then(($nav) => {
      cy.wrap($nav).contains('Devices')
      cy.wrap($nav).contains('Device Types')
      cy.wrap($nav).contains('Add Device')
      cy.wrap($nav).contains('Add Device Type')
      cy.wrap($nav).contains('Buildings')
      cy.wrap($nav).contains('Users')
      cy.wrap($nav).contains('Help')
    })
  })
})
