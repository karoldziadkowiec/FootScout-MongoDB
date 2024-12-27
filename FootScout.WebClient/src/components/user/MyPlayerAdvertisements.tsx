import React, { useEffect, useState } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import { Table, Button, Tabs, Tab } from 'react-bootstrap';
import { toast } from 'react-toastify';
import AccountService from '../../services/api/AccountService';
import UserService from '../../services/api/UserService';
import TimeService from '../../services/time/TimeService';
import PlayerAdvertisement from '../../models/interfaces/PlayerAdvertisement';
import '../../App.css';
import '../../styles/user/MyPlayerAdvertisements.css';

const MyPlayerAdvertisements = () => {
    const navigate = useNavigate();
    const location = useLocation();
    const [userActivePlayerAdvertisements, setUserActivePlayerAdvertisements] = useState<PlayerAdvertisement[]>([]);
    const [userInactivePlayerAdvertisements, setUserInactivePlayerAdvertisements] = useState<PlayerAdvertisement[]>([]);

    useEffect(() => {
        if (location.state && location.state.toastMessage)
            toast.success(location.state.toastMessage);

        const fetchUserPlayerAdvertisements = async () => {
            try {
                const userId = await AccountService.getId();
                if (userId) {
                    const _userActivePlayerAdvertisements = await UserService.getUserActivePlayerAdvertisements(userId);
                    setUserActivePlayerAdvertisements(_userActivePlayerAdvertisements);

                    const _userInactivePlayerAdvertisements = await UserService.getUserInactivePlayerAdvertisements(userId);
                    setUserInactivePlayerAdvertisements(_userInactivePlayerAdvertisements);
                }
            }
            catch (error) {
                console.error('Failed to fetch user\'s player advertisements:', error);
                toast.error('Failed to load user\'s player advertisements.');
            }
        };

        fetchUserPlayerAdvertisements();
    }, [location]);

    const moveToPlayerAdvertisementPage = (playerAdvertisementId: number) => {
        navigate(`/player-advertisement/${playerAdvertisementId}`, { state: { playerAdvertisementId } });
    };

    return (
        <div className="MyPlayerAdvertisements">
            <h1><i className="bi bi-person-bounding-box"></i> My Player Advertisements</h1>
            <Button variant="success" className="form-button" onClick={() => navigate('/new-player-advertisement')}>
                <i className="bi bi-file-earmark-plus-fill"></i>
                New Advertisement
            </Button>
            <p></p>
            {/* Active advertisements*/}
            <Tabs defaultActiveKey="active" id="problem-tabs" className="mb-3 custom-tabs">
                <Tab eventKey="active" title="Active advertisements">
                    <h3><i className="bi bi-bookmark-check"></i> Active advertisements</h3>
                    <div className="table-responsive">
                        <Table striped bordered hover variant="light">
                            <thead className="table-success">
                                <tr>
                                    <th>Creation Date</th>
                                    <th>Position</th>
                                    <th>Preferred League</th>
                                    <th>Region</th>
                                    <th>Salary (zł.) / month</th>
                                    <th></th>
                                </tr>
                            </thead>
                            <tbody>
                                {userActivePlayerAdvertisements.length > 0 ? (
                                    userActivePlayerAdvertisements.map((advertisement, index) => (
                                        <tr key={index}>
                                            <td>{TimeService.formatDateToEUR(advertisement.creationDate)}</td>
                                            <td>{advertisement.playerPosition.positionName}</td>
                                            <td>{advertisement.league}</td>
                                            <td>{advertisement.region}</td>
                                            <td>{advertisement.salaryRange.min} - {advertisement.salaryRange.max}</td>
                                            <td>
                                                <Button variant="dark" className="button-spacing" onClick={() => moveToPlayerAdvertisementPage(advertisement.id)}>
                                                    <i className="bi bi-info-square"></i>
                                                </Button>
                                            </td>
                                        </tr>
                                    ))
                                ) : (
                                    <tr>
                                        <td colSpan={9} className="text-center">No player advertisement available</td>
                                    </tr>
                                )}
                            </tbody>
                        </Table>
                    </div>
                </Tab>

                {/* Archived advertisements */}
                <Tab eventKey="archived" title="Archived advertisements">
                    <h3><i className="bi bi-clipboard-x"></i> Archived advertisements</h3>
                    <div className="table-responsive">
                        <Table striped bordered hover variant="light">
                            <thead className="table-warning">
                                <tr>
                                    <th>End Date (days ago)</th>
                                    <th>Position</th>
                                    <th>Preferred League</th>
                                    <th>Region</th>
                                    <th>Salary (zł.) / month</th>
                                    <th></th>
                                </tr>
                            </thead>
                            <tbody>
                                {userInactivePlayerAdvertisements.length > 0 ? (
                                    userInactivePlayerAdvertisements.map((advertisement, index) => (
                                        <tr key={index}>
                                            <td>{TimeService.formatDateToEUR(advertisement.endDate)} ({TimeService.calculateSkippedDays(advertisement.endDate)} days)</td>
                                            <td>{advertisement.playerPosition.positionName}</td>
                                            <td>{advertisement.league}</td>
                                            <td>{advertisement.region}</td>
                                            <td>{advertisement.salaryRange.min} - {advertisement.salaryRange.max}</td>
                                            <td>
                                                <Button variant="dark" className="button-spacing" onClick={() => moveToPlayerAdvertisementPage(advertisement.id)}>
                                                    <i className="bi bi-info-square"></i>
                                                </Button>
                                            </td>
                                        </tr>
                                    ))
                                ) : (
                                    <tr>
                                        <td colSpan={9} className="text-center">No player advertisement available</td>
                                    </tr>
                                )}
                            </tbody>
                        </Table>
                    </div>
                </Tab>
            </Tabs>
        </div>
    );
}

export default MyPlayerAdvertisements;