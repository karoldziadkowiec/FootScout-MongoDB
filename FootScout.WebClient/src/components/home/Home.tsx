import React, { useEffect } from 'react';
import { Link as RouterLink, useLocation, useNavigate } from 'react-router-dom';
import { toast } from 'react-toastify';
import { Element, scroller } from 'react-scroll';
import { Button, Container, Row, Col } from 'react-bootstrap';
import '../../App.css';
import '../../styles/home/Home.css';

const Home = () => {
    const navigate = useNavigate();
    const location = useLocation();
    
    useEffect(() => {
        if (location.state && location.state.toastMessage)
            toast.success(location.state.toastMessage);
        
    }, [location]);

    const handleLinkClick = (id: string) => {
        scroller.scrollTo(id, {
            smooth: true,
            duration: 100
        });
    };

    const moveToPage = (page: string) => {
        navigate(`/${page}`);
    };

    return (
        <div className="Home">
            <Element name="home" className="startSection">
                <div className="Home-logo-container">
                    <img src={require('../../img/logo.png')} alt="Home-logo" className="Home-logo" />
                    FootScout
                </div>
                <h2>YOUR BEST WEBISTE TO MANAGE YOUR FOOTBALL TRANSFERS</h2>
                <h4>Discover player/club offers and find the team/player that will be the best choice for you.</h4>
                <div className="links">
                    <RouterLink onClick={() => handleLinkClick("forPlayers")} to="#" className="link" style={{ cursor: "pointer" }}>For Players</RouterLink>
                    <div className="sign"> | </div>
                    <RouterLink onClick={() => handleLinkClick("forClubs")} to="#" className="link" style={{ cursor: "pointer" }}>For Clubs</RouterLink>
                </div>
            </Element>
            <Element name="forPlayers" className="blackSection">
                <h1><i className="bi bi-person-bounding-box"></i> For Players</h1>
                <h5>Find your dream club by searching ads and through recommendations and enjoy the games.</h5>
                <h5>Create a new ad as a player looking for a club and collect offers from clubs.</h5>
                <p></p>
                <Container className="links">
                    <Row>
                        <Col><Button variant="success" onClick={() => moveToPage("player-advertisements")}>Advertisements</Button></Col>
                        <Col><Button variant="success" onClick={() => moveToPage("new-player-advertisement")}>New Ad</Button></Col>
                    </Row>
                </Container>
            </Element>
            <Element name="forClubs" className="whiteSection">
                <h1><i className="bi bi-shield-fill"></i> For Clubs</h1>
                <h5>Find a dream player for your club by searching ads and through recommendations and enjoy the games.</h5>
                <h5>Add a new ad as a club looking for a player and collect offers from players.</h5>
                <p></p>
                <Container className="links">
                    <Row>
                        <Col><Button variant="success" onClick={() => moveToPage("club-advertisements")}>Advertisements</Button></Col>
                        <Col><Button variant="success" onClick={() => moveToPage("new-club-advertisement")}>New Ad</Button></Col>
                    </Row>
                </Container>
            </Element>
        </div>
    );
}

export default Home;