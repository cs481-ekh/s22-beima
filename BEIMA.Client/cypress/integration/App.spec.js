/// <reference types="cypress" />

describe('Open BEIMA Frontend', () => {
    it('Visits our running BEIMA Frontend', () => {
        cy.visit('http://localhost:3000')
        //cy.get('.App-link').should('have.attr', 'href', 'https://reactjs.org')
    })
})