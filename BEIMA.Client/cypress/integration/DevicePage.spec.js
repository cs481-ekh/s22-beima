/// <reference types="cypress" />

describe('Device Page', () => {
  it('Visits a Device Page', () => {
    // visit
    cy.visit('http://localhost:3000/devices/')
    cy.get('#' + Cypress.env('DEVICEID')).click();
    cy.log(Cypress.env('DEVICE_ID'));
    cy.get('[id=devicePageContent]').should('exist')
    cy.get('[id=itemCard]').should('exist')

    // wait for loading to finish
    cy.wait(10000)
    
    // not exists
    cy.get('[id=savebtn]').should('not.exist')
    cy.get('[id=cancelbtn]').should('not.exist')
    cy.get('[id=imageUpload]').should('not.exist')
    cy.get('[id=fileUpload]').should('not.exist')

    // exist
    cy.get('[id=editbtn]').should('exist')
    cy.get('[id=deletebtn]').should('exist')
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
    cy.visit('http://localhost:3000/devices/')
    cy.get('#' + Cypress.env('DEVICEID')).click();
    cy.get('[id=devicePageContent]').should('exist')
    cy.get('[id=itemCard]').should('exist')

    // wait for loading to finish
    cy.wait(10000)

    // click
    cy.get('[id=editbtn]').click()

    // not exists
    cy.get('[id=editbtn]').should('not.exist')

    // exists
    cy.get('[id=savebtn]').should('exist')
    cy.get('[id=cancelbtn]').should('exist')
    cy.get('[id=deletebtn]').should('exist')
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
  it('Resets fields on Cancel Button Click', () => {
    cy.visit('http://localhost:3000/devices/')
    cy.get('#' + Cypress.env('DEVICEID')).click();
    cy.wait(10000)
    cy.get('[id=editbtn]').click()

    // Set fields
    cy.get('[id=deviceNotes]').scrollIntoView().clear().type("Test Notes")
    cy.get('[id=deviceBuildingId]').scrollIntoView().clear().type("Test BId")
    cy.get('[id=deviceLatitude]').scrollIntoView().clear().type("Test Lat")
    cy.get('[id=deviceLongitude]').scrollIntoView().clear().type("Test Long")
    cy.get('[id=locationNotes]').scrollIntoView().clear().type("Test Notes")
    cy.get('[id=deviceTag]').scrollIntoView().clear().type("Test Tag")
    cy.get('[id=deviceModelNumber]').scrollIntoView().clear().type("Test Model Nbr")
    cy.get('[id=deviceSerialNumber]').scrollIntoView().clear().type("Test Ser Nbr")
    cy.get('[id=deviceManufacturer]').scrollIntoView().clear().type("Test Manu")
    cy.get('[id=deviceYearManufactured]').scrollIntoView().clear().type("Test Year Manu")

    // Set input
    cy.get('[id=deviceNotes]').should('have.value', 'Test Notes')
    cy.get('[id=deviceBuildingId]').should('have.value', 'Test BId')
    cy.get('[id=deviceLatitude]').should('have.value', 'Test Lat')
    cy.get('[id=deviceLongitude]').should('have.value', 'Test Long')
    cy.get('[id=locationNotes]').should('have.value', 'Test Notes')
    cy.get('[id=deviceTag]').should('have.value', 'Test Tag')
    cy.get('[id=deviceModelNumber]').should('have.value', 'Test Model Nbr')
    cy.get('[id=deviceSerialNumber]').should('have.value', 'Test Ser Nbr')
    cy.get('[id=deviceManufacturer]').should('have.value', 'Test Manu')
    cy.get('[id=deviceYearManufactured]').should('have.value', 'Test Year Manu')

    cy.get('[id=cancelbtn]').click()

    cy.get('[id=deviceNotes]').should('not.have.value', 'Test Notes')
    cy.get('[id=deviceBuildingId]').should('not.have.value', 'Test BId')
    cy.get('[id=deviceLatitude]').should('not.have.value', 'Test Lat')
    cy.get('[id=deviceLongitude]').should('not.have.value', 'Test Long')
    cy.get('[id=locationNotes]').should('not.have.value', 'Test Notes')
    cy.get('[id=deviceTag]').should('not.have.value', 'Test Tag')
    cy.get('[id=deviceModelNumber]').should('not.have.value', 'Test Model Nbr')
    cy.get('[id=deviceSerialNumber]').should('not.have.value', 'Test Ser Nbr')
    cy.get('[id=deviceManufacturer]').should('not.have.value', 'Test Manu')
    cy.get('[id=deviceYearManufactured]').should('not.have.value', 'Test Year Manu')
  })
})