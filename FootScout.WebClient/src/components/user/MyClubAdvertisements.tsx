import React, { useEffect, useState } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import { Table, Button, Tabs, Tab } from 'react-bootstrap';
import { toast } from 'react-toastify';
import AccountService from '../../services/api/AccountService';
import UserService from '../../services/api/UserService';
import ClubAdvertisement from '../../models/interfaces/ClubAdvertisement';
import '../../App.css';
import '../../styles/user/MyClubAdvertisements.css';
import TimeService from '../../services/time/TimeService';

const MyClubAdvertisements = () => {
    const navigate = useNavigate();
    const location = useLocation();
    const [userActiveClubAdvertisements, setUserActiveClubAdvertisements] = useState<ClubAdvertisement[]>([]);
    const [userInactiveClubAdvertisements, setUserInactiveClubAdvertisements] = useState<ClubAdvertisement[]>([]);

    useEffect(() => {
        if (location.state && location.state.toastMessage)
            toast.success(location.state.toastMessage);

        const fetchUserClubAdvertisements = async () => {
            try {
                const userId = await AccountService.getId();
                if (userId) {
                    const _userActiveClubAdvertisements = await UserService.getUserActiveClubAdvertisements(userId);
                    setUserActiveClubAdvertisements(_userActiveClubAdvertisements);

                    const _userInactiveClubAdvertisements = await UserService.getUserInactiveClubAdvertisements(userId);
                    setUserInactiveClubAdvertisements(_userInactiveClubAdvertisements);
                }
            }
            catch (error) {
                console.error('Failed to fetch user\'s club advertisements:', error);
                toast.error('Failed to load user\'s club advertisements.');
            }
        };

        fetchUserClubAdvertisements();
    }, [location]);

    const moveToClubAdvertisementPage = (clubAdvertisementId: number) => {
        navigate(`/club-advertisement/${clubAdvertisementId}`, { state: { clubAdvertisementId } });
    };

    return (
        <div className="MyClubAdvertisements">
            <h1><i className="bi bi-shield-fill"></i> My Club Advertisements</h1>
            <Button variant="success" className="form-button" onClick={() => navigate('/new-club-advertisement')}>
                <i className="bi bi-file-earmark-plus-fill"></i>
                New Advertisement
            </Button>
            <p></p>
            {/* Active advertisements*/}
            <Tabs defaultActiveKey="active" id="problem-tabs" className="mb-3 custom-tabs">
                <Tab eventKey="active" title="Active advertisements">
                    <h3><i className="bi bi-bookmark-check-fill"></i> Active advertisements</h3>
                    <div className="table-responsive">
                        <Table striped bordered hover variant="light">
                            <thead className="table-success">
                                <tr>
                                    <th>Creation Date</th>
                                    <th>Position</th>
                                    <th>Club Name</th>
                                    <th>League (Region)</th>
                                    <th>Salary (zł.) / month</th>
                                    <th></th>
                                </tr>
                            </thead>
                            <tbody>
                                {userActiveClubAdvertisements.length > 0 ? (
                                    userActiveClubAdvertisements.map((advertisement, index) => (
                                        <tr key={index}>
                                            <td>{TimeService.formatDateToEUR(advertisement.creationDate)}</td>
                                            <td>{advertisement.playerPosition.positionName}</td>
                                            <td>{advertisement.clubName}</td>
                                            <td>{advertisement.league} ({advertisement.region})</td>
                                            <td>{advertisement.salaryRange.min} - {advertisement.salaryRange.max}</td>
                                            <td>
                                                <Button variant="dark" className="button-spacing" onClick={() => moveToClubAdvertisementPage(advertisement.id)}>
                                                    <i className="bi bi-info-square"></i>
                                                </Button>
                                            </td>
                                        </tr>
                                    ))
                                ) : (
                                    <tr>
                                        <td colSpan={9} className="text-center">No club advertisement available</td>
                                    </tr>
                                )}
                            </tbody>
                        </Table>
                    </div>
                </Tab>

                {/* Archived advertisements */}
                <Tab eventKey="archived" title="Archived advertisements">
                    <h3><i className="bi bi-clipboard-x-fill"></i> Archived advertisements</h3>
                    <div className="table-responsive">
                        <Table striped bordered hover variant="light">
                            <thead className="table-warning">
                                <tr>
                                    <th>End Date (days ago)</th>
                                    <th>Position</th>
                                    <th>Club Name</th>
                                    <th>League (Region)</th>
                                    <th>Salary (zł.) / month</th>
                                    <th></th>
                                </tr>
                            </thead>
                            <tbody>
                                {userInactiveClubAdvertisements.length > 0 ? (
                                    userInactiveClubAdvertisements.map((advertisement, index) => (
                                        <tr key={index}>
                                            <td>{TimeService.formatDateToEUR(advertisement.creationDate)} ({TimeService.calculateSkippedDays(advertisement.endDate)} days)</td>
                                            <td>{advertisement.playerPosition.positionName}</td>
                                            <td>{advertisement.clubName}</td>
                                            <td>{advertisement.league} ({advertisement.region})</td>
                                            <td>{advertisement.salaryRange.min} - {advertisement.salaryRange.max}</td>
                                            <td>
                                                <Button variant="dark" className="button-spacing" onClick={() => moveToClubAdvertisementPage(advertisement.id)}>
                                                    <i className="bi bi-info-square"></i>
                                                </Button>
                                            </td>
                                        </tr>
                                    ))
                                ) : (
                                    <tr>
                                        <td colSpan={9} className="text-center">No club advertisement available</td>
                                    </tr>
                                )}
                            </tbody>
                        </Table>
                    </div>
                </Tab>
            </Tabs>
        </div >
    );
}

export default MyClubAdvertisements;