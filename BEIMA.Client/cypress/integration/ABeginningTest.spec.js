/// <reference types="cypress" />

/// The purpose of this test is to login to BEIMA before all the other tests are ran
/// to ensure that navigating through BEIMA can still happen.
/// The test database must have a user with this username, but the tester needs to replace the empty
/// password with a valid one.
describe("Login to BEIMA", () => {
  it('Login to BEIMA', () => {
    cy.visit('http://localhost:3000/')
    cy.get('[id=username]').scrollIntoView().clear().type("testuser")

    // REPLACE PASSWORD WITH VALID PASSWORD FROM TEST USER IN DATABASE
    cy.get('[id=password]').scrollIntoView().clear().type("testUser1!")
    cy.get('[id=submitBtn]').click()

    cy.wait(700);
  })
})