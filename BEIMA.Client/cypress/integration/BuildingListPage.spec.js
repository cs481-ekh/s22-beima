/// <reference types="cypress" />

describe('Building List Page', () => {
    it('Visits the Buildings Page', () => {
        cy.visit('http://localhost:3000/buildings')
        cy.get('#buildingListContent').should('exist')
        cy.get('#itemList').should('exist')
        cy.get('#addNewBuilding').should('exist')
    })
  })