/// <reference types="cypress" />

describe("Router Doesn't Redirect On Valid Pages", () =>{
  it('Visits Root', () =>{
    cy.visit('http://localhost:3000')
    cy.url().should('eq','http://localhost:3000/')
  })
  it('Visist Help Page', () => {
    cy.visit('http://localhost:3000/help')
    cy.url().should('include', '/help')
  })
})

describe("Router Redirects On Invalid Pages", () => {
  it('Visits Invalid Page', () => {
    cy.visit('http://localhost:3000/h3lp')
    cy.url().should('eq','http://localhost:3000/')
  })
  it('Visits /', () => {
    cy.visit('http://localhost:3000/')
    cy.url().should('eq','http://localhost:3000/')
  })
})