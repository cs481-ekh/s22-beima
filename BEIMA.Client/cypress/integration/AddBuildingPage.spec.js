/// <reference types="cypress" />
import { skipOn } from '@cypress/skip-test';


describe("Verify Buttons on Add Building Page", () => {
  it('Check for Add Device Button', () => {
    skipOn('linux')
    cy.visit('http://localhost:3000/buildings/addBuilding')
    cy.get("#addBuilding").contains('Add Building')
  })

  it('Check for Device Type DropDown', () => {
    skipOn('linux')
    cy.visit('http://localhost:3000/buildings/addBuilding')
    cy.get("#backBuildings")
  })
})

describe("Verify Data can be entered into fields", () => {
  it('Enter data into Building, Latitude, and Longitude fields', () => {
    skipOn('linux')
    cy.visit('http://localhost:3000/buildings/addBuilding')
    cy.get('#inputName').scrollIntoView().type("Student Union Building")
    cy.get('#inputLatitude').scrollIntoView().type("10.34452345")
    cy.get("#inputLongitude").scrollIntoView().type("12.355")
  })
})

describe("Verify Data in fields is cleared when Add Building is selected", () => {
  it('Enter data, click Add Building, verify fields are empty', () => {
    skipOn('linux')
    cy.visit('http://localhost:3000/buildings/addBuilding')
    cy.get('#inputName').scrollIntoView().type("Student Union Building")
    cy.get('#inputLatitude').scrollIntoView().type("10.34452345")
    cy.get("#inputLongitude").scrollIntoView().type("12.355")
    cy.get("#addBuilding").scrollIntoView().click()
    cy.get('#inputName').should('have.value', '')
    cy.get('#inputLatitude').should('have.value', '')
    cy.get("#inputLongitude").should('have.value', '')
  })
})

describe("Verify Data in fields is still present when invalid coords exist and Add Device is selected", () => {
  it('Enter data, click Add Device, verify fields are empty', () => {
    skipOn('linux')
    cy.visit('http://localhost:3000/buildings/addBuilding')
    cy.get('#inputName').scrollIntoView().type("Student Union Building")
    cy.get('#inputLatitude').scrollIntoView().type("200")
    cy.get('#inputLongitude').scrollIntoView().type("200")
    cy.get("#addBuilding").scrollIntoView().click()
    cy.get('#inputName').should('have.value', 'Student Union Building')
    cy.get('#inputLatitude').should('have.value', '200')
    cy.get('#inputLongitude').should('have.value', '200')
  })
})

describe("Verify the max character length of 1024", function () {
    it('Insert more than 1024 chars into input field, verify only 1024 are there', function (){
      skipOn('linux')
      
      cy.visit('http://localhost:3000/buildings/addBuilding')
      cy.get('#inputName').scrollIntoView().type(randomString1024())
      cy.get('#inputName').should('not.include.value', 'This text should not be included')
      
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