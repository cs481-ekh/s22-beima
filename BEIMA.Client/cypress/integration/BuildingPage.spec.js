/// <reference types="cypress" />

describe('Building Page', () => {
  it('Visits a Building Page', () => {
    // WHEN TESTING, CHANGE THIS TO A VALID BUILDING ID THAT IS IN THE DATABASE
    let buildingID = ''

    // visit
    cy.visit('http://localhost:3000/buildings/' + buildingID)
    cy.get('[id=buildingPageContent]').should('exist')
    cy.get('[id=itemCard]').should('exist')

    // wait for loading to finish
    cy.wait(2000)
    
    // not exists
    cy.get('[id=savebtn]').should('not.exist')
    cy.get('[id=cancelbtn]').should('not.exist')

    // exist
    cy.get('[id=editbtn]').should('exist')
    cy.get('[id=deletebtn]').should('exist')
    cy.get('[id=buildingName]').should('exist')
    cy.get('[id=buildingNotes]').should('exist')
    cy.get('[id=buildingNumber]').should('exist')
    cy.get('[id=buildingLatitude]').should('exist')
    cy.get('[id=buildingLongitude]').should('exist')

    // disabled
    cy.get('[id=buildingName]').should('be.disabled')
    cy.get('[id=buildingNotes]').should('be.disabled')
    cy.get('[id=buildingNumber]').should('be.disabled')
    cy.get('[id=buildingLatitude]').should('be.disabled')
    cy.get('[id=buildingLongitude]').should('be.disabled')
  })
  it('Enables inputs on Edit Button Click', () => {
    // WHEN TESTING, CHANGE THIS TO A VALID BUILDING ID THAT IS IN THE DATABASE
    let buildingID = ''

    // visit
    cy.visit('http://localhost:3000/buildings/' + buildingID)
    cy.get('[id=buildingPageContent]').should('exist')
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

    // enabled
    cy.get('[id=buildingName]').should('be.enabled')
    cy.get('[id=buildingNotes]').should('be.enabled')
    cy.get('[id=buildingNumber]').should('be.enabled')
    cy.get('[id=buildingLatitude]').should('be.enabled')
    cy.get('[id=buildingLongitude]').should('be.enabled')
  })
  it('Resets fields on Cancel Button Click', () => {
    // WHEN TESTING, CHANGE THIS TO A VALID BUILDING ID THAT IS IN THE DATABASE
    let buildingID = ''

    cy.visit('http://localhost:3000/buildings/' + buildingID)
    cy.wait(2000)
    cy.get('[id=editbtn]').click()

    // Set fields
    cy.get('[id=buildingNotes]').scrollIntoView().clear().type("Test Notes")
    cy.get('[id=buildingLatitude]').scrollIntoView().clear().type("Test Lat")
    cy.get('[id=buildingLongitude]').scrollIntoView().clear().type("Test Long")
    cy.get('[id=buildingNumber]').scrollIntoView().clear().type("Test Number")

    // Validate input
    cy.get('[id=buildingNotes]').should('have.value', 'Test Notes')
    cy.get('[id=buildingLatitude]').should('have.value', 'Test Lat')
    cy.get('[id=buildingLongitude]').should('have.value', 'Test Long')
    cy.get('[id=buildingNumber]').should('have.value', 'Test Number')

    cy.get('[id=cancelbtn]').click()

    cy.get('[id=buildingNotes]').should('not.have.value', 'Test Notes')
    cy.get('[id=buildingLatitude]').should('not.have.value', 'Test Lat')
    cy.get('[id=buildingLongitude]').should('not.have.value', 'Test Long')
    cy.get('[id=buildingNumber]').should('not.have.value', 'Test Number')
  })
})

describe("Verify the max character length of 1024", function () {
  it('Insert more than 1024 chars into input field, verify only 1024 are there', function (){
    // WHEN TESTING, CHANGE THIS TO A VALID BUILDING ID THAT IS IN THE DATABASE
    let buildingID = ''

    cy.visit('http://localhost:3000/buildings/' + buildingID)
    cy.wait(2000)
    cy.get('[id=editbtn]').click()
    cy.get('[id=buildingNotes]').scrollIntoView().type(randomString1024())
    cy.get('[id=buildingNotes]').should('not.include.value', 'This text should not be included')

    function randomString1024() {
      var text = "";
      var possible = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
  
      for (var i = 0; i < 1024; i++){
        text += possible.charAt(Math.floor(Math.random() * possible.length));
      }
      text += "This text should not be included";

      return text;
    }
  })
})