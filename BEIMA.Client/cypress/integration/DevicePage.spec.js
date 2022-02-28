/// <reference types="cypress" />

describe('Device Page', () => {
  it('Visits a Device Page', () => {
    // visit
    cy.visit('http://localhost:3000/devices/5')
    cy.get('[id=devicePageContent]').should('exist')
    cy.get('[id=itemCard]').should('exist')

    // wait for loading to finish
    cy.wait(1250)
    
    // not exists
    cy.get('[id=savebtn]').should('not.exist')
    cy.get('[id=cancelbtn]').should('not.exist')
    cy.get('[id=imageUpload]').should('not.exist')
    cy.get('[id=fileUpload]').should('not.exist')

    // exist
    cy.get('[id=editbtn]').should('exist')
    cy.get('[id=imageDisplay]').should('exist')
    cy.get('[id=documents]').should('exist')
    cy.get('[id=deviceNotes]').should('exist')
    cy.get('[id=deviceBuildingId]').should('exist')
    cy.get('[id=deviceLatitude]').should('exist')
    cy.get('[id=deviceLongitude]').should('exist')
    cy.get('[id=locationNotes]').should('exist')
    cy.get('[id=deviceTag]').should('exist')
    cy.get('[id=deviceModelNumber]').should('exist')
    cy.get('[id=deviceSerialNumber]').should('exist')
    cy.get('[id=deviceManufacturer]').should('exist')
    cy.get('[id=deviceYearManufactured]').should('exist')

    // disabled
    cy.get('[id=deviceNotes]').should('be.disabled')
    cy.get('[id=deviceBuildingId]').should('be.disabled')
    cy.get('[id=deviceLatitude]').should('be.disabled')
    cy.get('[id=deviceLongitude]').should('be.disabled')
    cy.get('[id=locationNotes]').should('be.disabled')
    cy.get('[id=deviceTag]').should('be.disabled')
    cy.get('[id=deviceModelNumber]').should('be.disabled')
    cy.get('[id=deviceSerialNumber]').should('be.disabled')
    cy.get('[id=deviceManufacturer]').should('be.disabled')
    cy.get('[id=deviceYearManufactured]').should('be.disabled')
  })
  it('Enables inputs on Edit Button Click', () => {
    // visit
    cy.visit('http://localhost:3000/devices/5')
    cy.get('[id=devicePageContent]').should('exist')
    cy.get('[id=itemCard]').should('exist')

    // wait for loading to finish
    cy.wait(1250)

    // click
    cy.get('[id=editbtn]').click()

    // not exists
    cy.get('[id=editbtn]').should('not.exist')

    // exists
    cy.get('[id=savebtn]').should('exist')
    cy.get('[id=cancelbtn]').should('exist')
    cy.get('[id=imageUpload]').should('exist')
    cy.get('[id=fileUpload]').should('exist')

    // enabled
    cy.get('[id=imageUpload]').should('be.enabled')
    cy.get('[id=fileUpload]').should('be.enabled')
    cy.get('[id=deviceNotes]').should('be.enabled')
    cy.get('[id=deviceBuildingId]').should('be.enabled')
    cy.get('[id=deviceLatitude]').should('be.enabled')
    cy.get('[id=deviceLongitude]').should('be.enabled')
    cy.get('[id=locationNotes]').should('be.enabled')
    cy.get('[id=deviceTag]').should('be.enabled')
    cy.get('[id=deviceModelNumber]').should('be.enabled')
    cy.get('[id=deviceSerialNumber]').should('be.enabled')
    cy.get('[id=deviceManufacturer]').should('be.enabled')
    cy.get('[id=deviceYearManufactured]').should('be.enabled')
  })
})