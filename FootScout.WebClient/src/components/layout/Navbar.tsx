import React from 'react';
import Container from 'react-bootstrap/Container';
import Nav from 'react-bootstrap/Nav';
import Navbar from 'react-bootstrap/Navbar';
import NavDropdown from 'react-bootstrap/NavDropdown';
import { NavLink } from 'react-router-dom';
import AccountService from '../../services/api/AccountService';
import CurrentTimeDisplay from '../../services/time/CurrentTimeDisplay';
import '../../App.css';
import '../../styles/layout/Navbar.css';

const NavbarComponent = () => {
  return (
    <Navbar bg="dark" variant="dark" expand="lg" className="sticky-top">
      <Container>
        <img src={require('../../img/logo.png')} alt="logo" className="logo" />
        <Navbar.Brand as={NavLink} to="/home">FootScout</Navbar.Brand>
        <Navbar.Toggle aria-controls="responsive-navbar-nav" />
        <Navbar.Collapse id="responsive-navbar-nav">
          <Nav className="me-auto green-links">
            <Nav.Link as={NavLink} to="/home"><i className="bi bi-house-fill"></i> Home</Nav.Link>
            <NavDropdown title={<><i className="bi bi-list-nested"></i> Advertisements</>} id="basic-nav-dropdown">
              <NavDropdown.Item as={NavLink} to="/player-advertisements"><i className="bi bi-person-bounding-box"></i> Player's</NavDropdown.Item>
              <NavDropdown.Item as={NavLink} to="/club-advertisements"><i className="bi bi-shield-fill"></i> Club's</NavDropdown.Item>
            </NavDropdown>
            <NavDropdown title={<><i className="bi bi-file-earmark-plus"></i> New Advertisement</>} id="basic-nav-dropdown">
              <NavDropdown.Item as={NavLink} to="/new-player-advertisement"><i className="bi bi-person-bounding-box"></i> as Player</NavDropdown.Item>
              <NavDropdown.Item as={NavLink} to="/new-club-advertisement"><i className="bi bi-shield-fill"></i> as Club</NavDropdown.Item>
            </NavDropdown>
          </Nav>
          <Nav className="ms-auto green-links">
            <Nav.Link as={NavLink} to="/chats"><i className="bi bi-chat-fill"></i> Chat </Nav.Link>
            <NavDropdown title={<><i className="bi bi-briefcase-fill"></i> My Offers</>} id="basic-nav-dropdown">
              <NavDropdown.Item as={NavLink} to="/my-offers-as-player"><i className="bi bi-person-bounding-box"></i> as Player</NavDropdown.Item>
              <NavDropdown.Item as={NavLink} to="/my-offers-as-club"><i className="bi bi-shield-fill"></i> as Club</NavDropdown.Item>
            </NavDropdown>
            <NavDropdown title={<><i className="bi bi-person-circle"></i> My Profile</>} id="basic-nav-dropdown" className="sidebar-dropdown">              
              <NavDropdown.Item as={NavLink} to="/my-profile"><i className="bi bi-person-fill"></i> Profile</NavDropdown.Item>
              <NavDropdown.Item as={NavLink} to="/club-history"><i className="bi bi-clock-history"></i> Club History</NavDropdown.Item>
              <NavDropdown.Item as={NavLink} to="/my-player-advertisements"><i className="bi bi-person-bounding-box"></i> Player Ads</NavDropdown.Item>
              <NavDropdown.Item as={NavLink} to="/my-club-advertisements"><i className="bi bi-shield-fill"></i> Club Ads</NavDropdown.Item>
              <NavDropdown.Item as={NavLink} to="/my-favorite-player-advertisements"><i className="bi bi-chat-square-heart"></i> Favorite Player Ads</NavDropdown.Item>
              <NavDropdown.Item as={NavLink} to="/my-favorite-club-advertisements"><i className="bi bi-chat-square-heart-fill"></i> Favorite Club Ads</NavDropdown.Item>
              <NavDropdown.Item as={NavLink} to="/support"><i className="bi bi-wrench-adjustable"></i> Support</NavDropdown.Item>
              <NavDropdown.Item onClick={AccountService.logout} as={NavLink} to="/"><i className="bi bi-box-arrow-left"></i> Log out</NavDropdown.Item>
            </NavDropdown>
            <Navbar.Text> <CurrentTimeDisplay/> </Navbar.Text>
          </Nav>
        </Navbar.Collapse>
      </Container>
    </Navbar>
  );
}

export default NavbarComponent;