/// <reference types="cypress" />

describe('Device Types Page', () => {
  it('Visits the Device Types Page', () => {
      cy.visit('http://localhost:3000/deviceTypes')
      cy.get('[id=deviceTypesContent]').should('exist')
      cy.get('[id=itemList]').should('exist')
  })
})

describe("Verify the list has items present", function () {
  it('Count children of item list (see comments)', function (){
    cy.visit('http://localhost:3000/deviceTypes')

    //get all elements on the page with a class that starts with "ItemList_item"
    //if there's at least one device in the DB it will pass since 2 elements with
    //the expected class are created for each device listing
    cy.get('[class^="ItemList_item"]').its('length').should('be.gt', 2);
  })
})