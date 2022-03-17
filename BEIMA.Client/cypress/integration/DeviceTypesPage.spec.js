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

    //get all children of the itemlist with class ItemList_item__2BOfJ
    //there should be at least 1 item in the list
    cy.get('[id=itemList]').find('.ItemList_item__2BOfJ').its('length').should('be.gt', 0);

  })
})