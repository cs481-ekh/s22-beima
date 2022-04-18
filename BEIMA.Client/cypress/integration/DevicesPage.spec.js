/// <reference types="cypress" />

describe('Devices Page', () => {
  beforeEach(() => {
    cy.visit('http://localhost:3000/devices')
  })

  it('Visits the Devices Page', () => {
      cy.get('[id=devicesPageContent]').should('exist')
      cy.get('[id=itemList]').should('exist')

      cy.get('[id=buildingFilter]').should('exist')
      cy.get('[id=deviceTypeFilter]').should('exist')
      cy.get('[id=clearFilterButton]').should('exist')
      cy.get('[id=submitFilterButton]').should('exist')
  })

  it('Can interact with device type filter', () => {
    // Assumes that there are at least two devices in the db that have different device types. If not add them before running the tests.
    cy.get('*[data-cy=listItem]').then((items) => {
      const itemListCount = items.length

      cy.get('#deviceTypeFilter').click().type('{enter}').type('{esc}')
      cy.get('#submitFilterButton').click()

      cy.wait(1000)
      cy.get('*[data-cy=listItem]').should((items) => {
        expect(items.length).to.lt(itemListCount)
      })
    })
  })

  it('Can interact with building filter', () => {
    // Assumes that there are at least two devices in the db that have different buildings. If not add them before running the tests.
    cy.get('*[data-cy=listItem]').then((items) => {
      const itemListCount = items.length

      cy.get('#buildingFilter').click().type('{enter}').type('{esc}')
      cy.get('#submitFilterButton').click()

      cy.wait(1000)
      cy.get('*[data-cy=listItem]').should((items) => {
        expect(items.length).to.lt(itemListCount)
      })
    })
  })

  it('Resets when Clear Filters is presssed', () => {
    // Assumes that there are at least two devices in the db that have different device types. If not add them before running the tests.
    cy.get('*[data-cy=listItem]').then((items) => {
      const itemListCount = items.length

      cy.get('#deviceTypeFilter').click().type('{enter}').type('{esc}')
      cy.get('#submitFilterButton').click()

      cy.wait(1000)
      cy.get('*[data-cy=listItem]').should((items) => {
        expect(items.length).to.lt(itemListCount)
      })

      cy.get('#clearFilterButton').click()
      cy.wait(1000)

      cy.get('*[data-cy=listItem]').should((items) => {
        expect(items.length).to.eq(itemListCount)
      })
    })
  })
})