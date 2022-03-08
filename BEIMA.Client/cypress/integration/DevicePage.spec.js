/// <reference types="cypress" />
import { skipOn } from '@cypress/skip-test'

describe('Device Page', () => {
  it('Visits a Device Page', () => {
    skipOn('linux')
    // visit
    cy.visit('http://localhost:3000/devices/' + Cypress.env('DEVICEID'))
    cy.get('[id=devicePageContent]').should('exist')
    cy.get('[id=itemCard]').should('exist')

    // wait for loading to finish
    cy.wait(2000)
    
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

    cy.get('[id=fields]').within(() => {
      cy.get('input').each((val, index, collection) => {
        cy.wrap(val).should('exist')
      })
    })

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
    cy.get('[id=fields]').within(() => {
      cy.get('input').each((val, index, collection) => {
        cy.wrap(val).should('be.disabled')
      })
    })
  })
  it('Enables inputs on Edit Button Click', () => {
    skipOn('linux')
    // visit
    cy.visit('http://localhost:3000/devices/' + Cypress.env('DEVICEID'))
    cy.get('[id=devicePageContent]').should('exist')
    cy.get('[id=itemCard]').should('exist')

    // wait for loading to finish
    cy.wait(2000)

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
    cy.get('[id=fields]').within(() => {
      cy.get('input').each((val, index, collection) => {
        cy.wrap(val).should('be.enabled')
      })
    })
  })
  it('Resets fields on Cancel Button Click', () => {
    skipOn('linux')
    cy.visit('http://localhost:3000/devices/' + Cypress.env('DEVICEID'))
    cy.wait(2000)
    cy.get('[id=editbtn]').click()

    // Set fields
    cy.get('[id=deviceNotes]').scrollIntoView().clear().type("Test Notes")
    cy.get('[id=deviceBuildingId]').scrollIntoView().clear().type("Test BId")
    cy.get('[id=deviceLatitude]').scrollIntoView().clear().type("Test Lat")
    cy.get('[id=deviceLongitude]').scrollIntoView().clear().type("Test Long")
    cy.get('[id=locationNotes]').scrollIntoView().clear().type("Test Notes")

    cy.get('[id=fields]').within(() => {
      cy.get('input').each((val, index, collection) => {
        console.log(val)
        cy.wrap(val).scrollIntoView().clear().type("Test" + index)
      })
    })

    // Validate input
    cy.get('[id=deviceNotes]').should('have.value', 'Test Notes')
    cy.get('[id=deviceBuildingId]').should('have.value', 'Test BId')
    cy.get('[id=deviceLatitude]').should('have.value', 'Test Lat')
    cy.get('[id=deviceLongitude]').should('have.value', 'Test Long')
    cy.get('[id=locationNotes]').should('have.value', 'Test Notes')
    cy.get('[id=deviceTag]').should('have.value', 'Test0')
    cy.get('[id=deviceModelNumber]').should('have.value', 'Test1')
    cy.get('[id=deviceSerialNumber]').should('have.value', 'Test2')
    cy.get('[id=deviceManufacturer]').should('have.value', 'Test3')

    cy.get('[id=fields]').within(() => {
      cy.get('input').each((val, index, collection) => {
        cy.wrap(val).should('have.value', 'Test' + index)
      })
    })

    cy.get('[id=cancelbtn]').click()

    cy.get('[id=deviceNotes]').should('not.have.value', 'Test Notes')
    cy.get('[id=deviceBuildingId]').should('not.have.value', 'Test BId')
    cy.get('[id=deviceLatitude]').should('not.have.value', 'Test Lat')
    cy.get('[id=deviceLongitude]').should('not.have.value', 'Test Long')
    cy.get('[id=locationNotes]').should('not.have.value', 'Test Notes')
    cy.get('[id=deviceTag]').should('not.have.value', 'Test0')
    cy.get('[id=deviceModelNumber]').should('not.have.value', 'Test1')
    cy.get('[id=deviceSerialNumber]').should('not.have.value', 'Test2')
    cy.get('[id=deviceManufacturer]').should('not.have.value', 'Test3')

    cy.get('[id=fields]').within(() => {
      cy.get('input').each((val, index, collection) => {
        cy.wrap(val).should('not.have.value', 'Test' + index)
      })
    })
  })
})