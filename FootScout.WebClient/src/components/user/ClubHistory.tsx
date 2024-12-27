import React, { useEffect, useState } from 'react';
import { Table, Form, Button, Row, Col, Modal, FormSelect } from 'react-bootstrap';
import { toast } from 'react-toastify';
import AccountService from '../../services/api/AccountService';
import UserService from '../../services/api/UserService';
import TimeService from '../../services/time/TimeService';
import ClubHistoryService from '../../services/api/ClubHistoryService';
import PlayerPositionService from '../../services/api/PlayerPositionService';
import ClubHistoryModel from '../../models/interfaces/ClubHistory';
import PlayerPosition from '../../models/interfaces/PlayerPosition';
import UserDTO from '../../models/dtos/UserDTO';
import ClubHistoryCreateDTO from '../../models/dtos/ClubHistoryCreateDTO';
import '../../App.css';
import '../../styles/user/ClubHistory.css';

const ClubHistory = () => {
    const [user, setUser] = useState<UserDTO | null>(null);
    const [userClubHistories, setUserClubHistories] = useState<ClubHistoryModel[]>([]);
    const [positions, setPositions] = useState<PlayerPosition[]>([]);
    const [showCreateModal, setShowCreateModal] = useState<boolean>(false);
    const [showDetailsModal, setShowDetailsModal] = useState<boolean>(false);
    const [showEditModal, setShowEditModal] = useState<boolean>(false);
    const [showDeleteModal, setShowDeleteModal] = useState<boolean>(false);
    const [createFormData, setCreateFormData] = useState<ClubHistoryCreateDTO>({
        playerPositionId: 0,
        clubName: '',
        league: '',
        region: '',
        startDate: '',
        endDate: '',
        achievements: {
            numberOfMatches: 0,
            goals: 0,
            assists: 0,
            additionalAchievements: '',
        },
        playerId: ''
    });
    const [selectedClubHistory, setSelectedClubHistory] = useState<ClubHistoryModel | null>(null);
    const [editFormData, setEditFormData] = useState<ClubHistoryModel | null>(null);
    const [deleteHistoryId, setDeleteHistoryId] = useState<number | null>(null);

    useEffect(() => {
        const fetchUserData = async () => {
            try {
                const userId = await AccountService.getId();
                if (userId) {
                    const userData = await UserService.getUser(userId);
                    setUser(userData);

                    const _userClubHistories = await UserService.getUserClubHistory(userId);
                    setUserClubHistories(_userClubHistories);
                }
            }
            catch (error) {
                console.error('Failed to fetch user data:', error);
                toast.error('Failed to load user data.');
            }
        };

        const fetchPositions = async () => {
            try {
                const positionsData = await PlayerPositionService.getPlayerPositions();
                setPositions(positionsData);
            }
            catch (error) {
                console.error('Failed to fetch positions:', error);
                toast.error('Failed to load positions.');
            }
        };

        fetchUserData();
        fetchPositions();
    }, []);

    const handleCreateClubHistory = async () => {
        if (!user)
            return;

        const validationError = validateForm(createFormData);
        if (validationError) {
            toast.error(validationError);
            return;
        }

        try {
            const newFormData = { ...createFormData, playerId: user.id };

            await ClubHistoryService.createClubHistory(newFormData);
            setShowCreateModal(false);
            toast.success('Club history created successfully!');
            // Refresh the user data
            const _userClubHistories = await UserService.getUserClubHistory(user.id);
            setUserClubHistories(_userClubHistories);
        }
        catch (error) {
            console.error('Failed to create club history:', error);
            toast.error('Failed to create club history.');
        }
    };

    const validateForm = (formData: ClubHistoryCreateDTO | ClubHistoryModel) => {
        const { playerPositionId, clubName, league, region, startDate, endDate, achievements } = formData;
        const { numberOfMatches, goals, assists } = achievements;

        if (!playerPositionId || !clubName || !league || !region || !startDate || !endDate)
            return 'All fields are required.';

        if (isNaN(Number(numberOfMatches)) || isNaN(Number(goals)) || isNaN(Number(assists)))
            return 'Matches, goals, and assists must be numbers.';

        if (Number(numberOfMatches) < 0 || Number(goals) < 0 || Number(assists) < 0)
            return 'Matches, goals and assists must be greater than or equal to 0.';

        const start = new Date(startDate);
        const end = new Date(endDate);
        if (end <= start) {
            return 'End date must be later than start date.';
        }

        return null;
    };

    const handleShowDetails = (clubHistory: ClubHistoryModel) => {
        setSelectedClubHistory(clubHistory);
        setShowDetailsModal(true);
    };

    const handleShowEditModal = (clubHistory: ClubHistoryModel) => {
        setEditFormData(clubHistory);
        setShowEditModal(true);
    };

    const handleEditClubHistory = async () => {
        if (!user || !editFormData)
            return;

        const validationError = validateForm(editFormData);
        if (validationError) {
            toast.error(validationError);
            return;
        }

        try {
            const updatedFormData = {
                ...editFormData
            };

            await ClubHistoryService.updateClubHistory(editFormData.id, updatedFormData);
            setShowEditModal(false);
            toast.success('Club history updated successfully!');
            // Refresh the user data
            const _userClubHistories = await UserService.getUserClubHistory(user.id);
            setUserClubHistories(_userClubHistories);
        } catch (error) {
            console.error('Failed to update club history:', error);
            toast.error('Failed to update club history.');
        }
    };

    const handleShowDeleteModal = (clubHistoryId: number) => {
        setDeleteHistoryId(clubHistoryId);
        setShowDeleteModal(true);
    };

    const handleDeleteClubHistory = async () => {
        if (!user || !deleteHistoryId)
            return;

        try {
            await ClubHistoryService.deleteClubHistory(deleteHistoryId);
            toast.success('Your club history has been deleted successfully.');
            setShowDeleteModal(false);
            setDeleteHistoryId(null);
            // Refresh the user data
            const _userClubHistories = await UserService.getUserClubHistory(user.id);
            setUserClubHistories(_userClubHistories);
        }
        catch (error) {
            console.error('Failed to delete club history:', error);
            toast.error('Failed to delete club history.');
        }
    };

    return (
        <div className="ClubHistory">
            <h1><i className="bi bi-clock-history"></i> Club History</h1>
            <p></p>
            <Button variant="success" className="form-button" onClick={() => setShowCreateModal(true)}>
                <i className="bi bi-file-earmark-plus"></i>
                Create Club History
            </Button>
            <p></p>
            <div className="table-responsive">
                <Table striped bordered hover variant="warning">
                    <thead className="table-dark">
                        <tr>
                            <th>Date</th>
                            <th>Club</th>
                            <th>League</th>
                            <th>Region</th>
                            <th>Position</th>
                            <th></th>
                        </tr>
                    </thead>
                    <tbody>
                        {userClubHistories.length > 0 ? (
                            userClubHistories.map((history, index) => (
                                <tr key={index}>
                                    <td>{TimeService.formatDateToEUR(history.startDate)} - {TimeService.formatDateToEUR(history.endDate)}</td>
                                    <td>{history.clubName}</td>
                                    <td>{history.league}</td>
                                    <td>{history.region}</td>
                                    <td>{history.playerPosition.positionName}</td>
                                    <td>
                                        <Button variant="dark" className="button-spacing" onClick={() => handleShowDetails(history)}>
                                            <i className="bi bi-info-square"></i>
                                        </Button>
                                        <Button variant="warning" className="button-spacing" onClick={() => handleShowEditModal(history)}>
                                            <i className="bi bi-pencil-square"></i>
                                        </Button>
                                        <Button variant="danger" onClick={() => handleShowDeleteModal(history.id)}>
                                            <i className="bi bi-trash"></i>
                                        </Button>
                                    </td>
                                </tr>
                            ))
                        ) : (
                            <tr>
                                <td colSpan={9} className="text-center">No club history available</td>
                            </tr>
                        )}
                    </tbody>
                </Table>
            </div>

            {/* Create Club History Modal */}
            <Modal show={showCreateModal} onHide={() => setShowCreateModal(false)}>
                <Modal.Header closeButton>
                    <Modal.Title>Create Club History</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <Form>
                        <Form.Group as={Row} controlId="formPosition">
                            <Form.Label column sm="3">Position</Form.Label>
                            <Col sm="9">
                                <FormSelect
                                    value={createFormData.playerPositionId}
                                    onChange={(e) => setCreateFormData({
                                        ...createFormData,
                                        playerPositionId: parseInt(e.target.value, 10)
                                    })}
                                >
                                    <option value="">Select Position</option>
                                    {positions.map((position) => (
                                        <option key={position.id} value={position.id}>
                                            {position.positionName}
                                        </option>
                                    ))}
                                </FormSelect>
                            </Col>
                        </Form.Group>

                        <Form.Group as={Row} controlId="formClubName">
                            <Form.Label column sm="3">Club Name</Form.Label>
                            <Col sm="9">
                                <Form.Control
                                    type="text"
                                    placeholder="Club Name"
                                    value={createFormData.clubName}
                                    onChange={(e) => setCreateFormData({ ...createFormData, clubName: e.target.value })}
                                    maxLength={30}
                                    required
                                />
                            </Col>
                        </Form.Group>

                        <Form.Group as={Row} controlId="formLeague">
                            <Form.Label column sm="3">League</Form.Label>
                            <Col sm="9">
                                <Form.Control
                                    type="text"
                                    placeholder="League"
                                    value={createFormData.league}
                                    onChange={(e) => setCreateFormData({ ...createFormData, league: e.target.value })}
                                    maxLength={30}
                                    required
                                />
                            </Col>
                        </Form.Group>

                        <Form.Group as={Row} controlId="formRegion">
                            <Form.Label column sm="3">Region</Form.Label>
                            <Col sm="9">
                                <Form.Control
                                    type="text"
                                    placeholder="Region"
                                    value={createFormData.region}
                                    onChange={(e) => setCreateFormData({ ...createFormData, region: e.target.value })}
                                    maxLength={30}
                                    required
                                />
                            </Col>
                        </Form.Group>

                        <Form.Group as={Row} controlId="formStartDate">
                            <Form.Label column sm="3">Start Date</Form.Label>
                            <Col sm="9">
                                <Form.Control
                                    type="date"
                                    value={createFormData.startDate}
                                    onChange={(e) => setCreateFormData({ ...createFormData, startDate: e.target.value })}
                                    required
                                />
                            </Col>
                        </Form.Group>

                        <Form.Group as={Row} controlId="formEndDate">
                            <Form.Label column sm="3">End Date</Form.Label>
                            <Col sm="9">
                                <Form.Control
                                    type="date"
                                    value={createFormData.endDate}
                                    onChange={(e) => setCreateFormData({ ...createFormData, endDate: e.target.value })}
                                    required
                                />
                            </Col>
                        </Form.Group>

                        <Form.Group as={Row} controlId="formMatches">
                            <Form.Label column sm="3">Matches</Form.Label>
                            <Col sm="9">
                                <Form.Control
                                    type="number"
                                    placeholder="Number of Matches"
                                    value={createFormData.achievements.numberOfMatches}
                                    onChange={(e) => setCreateFormData({
                                        ...createFormData,
                                        achievements: {
                                            ...createFormData.achievements,
                                            numberOfMatches: parseInt(e.target.value)
                                        }
                                    })}
                                    required
                                />
                            </Col>
                        </Form.Group>

                        <Form.Group as={Row} controlId="formGoals">
                            <Form.Label column sm="3">Goals</Form.Label>
                            <Col sm="9">
                                <Form.Control
                                    type="number"
                                    placeholder="Goals"
                                    value={createFormData.achievements.goals}
                                    onChange={(e) => setCreateFormData({
                                        ...createFormData,
                                        achievements: {
                                            ...createFormData.achievements,
                                            goals: parseInt(e.target.value)
                                        }
                                    })}
                                    required
                                />
                            </Col>
                        </Form.Group>

                        <Form.Group as={Row} controlId="formAssists">
                            <Form.Label column sm="3">Assists</Form.Label>
                            <Col sm="9">
                                <Form.Control
                                    type="number"
                                    placeholder="Assists"
                                    value={createFormData.achievements.assists}
                                    onChange={(e) => setCreateFormData({
                                        ...createFormData,
                                        achievements: {
                                            ...createFormData.achievements,
                                            assists: parseInt(e.target.value)
                                        }
                                    })}
                                    required
                                />
                            </Col>
                        </Form.Group>

                        <Form.Group as={Row} controlId="formAdditionalAchievements">
                            <Form.Label column sm="3">Achievements</Form.Label>
                            <Col sm="9">
                                <Form.Control
                                    as="textarea"
                                    placeholder="Additional Achievements"
                                    value={createFormData.achievements.additionalAchievements}
                                    onChange={(e) => setCreateFormData({
                                        ...createFormData,
                                        achievements: {
                                            ...createFormData.achievements,
                                            additionalAchievements: e.target.value
                                        }
                                    })}
                                    maxLength={200}
                                />
                            </Col>
                        </Form.Group>
                    </Form>
                </Modal.Body>
                <Modal.Footer>
                    <Button variant="secondary" onClick={() => setShowCreateModal(false)}>Close</Button>
                    <Button variant="success" onClick={handleCreateClubHistory}>Create</Button>
                </Modal.Footer>
            </Modal>

            {/* Details of Club History Modal */}
            <Modal show={showDetailsModal} onHide={() => setShowDetailsModal(false)}>
                <Modal.Header closeButton>
                    <Modal.Title>Club History Details</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    {selectedClubHistory && (
                        <div className="modal-content-centered">
                            <p><Form.Label className="clubHistory-name-label">{(selectedClubHistory.clubName).toUpperCase()}</Form.Label></p>
                            <p><Form.Label className="clubHistory-position-label">{selectedClubHistory.playerPosition.positionName}</Form.Label></p>
                            <Row>
                                <Col>
                                    <Form.Label className="clubHistory-section">CLUB INFO</Form.Label>
                                    <p><strong>League:</strong> {selectedClubHistory.league}</p>
                                    <p><strong>Region:</strong> {selectedClubHistory.region}</p>
                                    <p><strong>Start Date:</strong> {TimeService.formatDateToEUR(selectedClubHistory.startDate)}</p>
                                    <p><strong>End Date:</strong> {TimeService.formatDateToEUR(selectedClubHistory.endDate)}</p>
                                </Col>
                                <Col>
                                    <Form.Label className="clubHistory-section">ACHIEVEMENTS</Form.Label>
                                    <p><strong>Matches:</strong> {selectedClubHistory.achievements.numberOfMatches}</p>
                                    <p><strong>Goals:</strong> {selectedClubHistory.achievements.goals}</p>
                                    <p><strong>Assists:</strong> {selectedClubHistory.achievements.assists}</p>
                                    <p><strong>Additional Achievements:</strong> {selectedClubHistory.achievements.additionalAchievements}</p>
                                </Col>
                            </Row>
                        </div>
                    )}
                </Modal.Body>
                <Modal.Footer>
                    <Button variant="secondary" onClick={() => setShowDetailsModal(false)}>Close</Button>
                </Modal.Footer>
            </Modal>

            {/* Edit Club History Modal */}
            {editFormData && (
                <Modal show={showEditModal} onHide={() => setShowEditModal(false)}>
                    <Modal.Header closeButton>
                        <Modal.Title>Edit Club History</Modal.Title>
                    </Modal.Header>
                    <Modal.Body>
                        <Form>
                            <Form.Group as={Row} controlId="formPosition">
                                <Form.Label column sm="3">Position</Form.Label>
                                <Col sm="9">
                                    <FormSelect
                                        value={editFormData.playerPositionId}
                                        onChange={(e) => setEditFormData({
                                            ...editFormData,
                                            playerPositionId: parseInt(e.target.value, 10)
                                        })}
                                    >
                                        <option value="">Select Position</option>
                                        {positions.map((position) => (
                                            <option key={position.id} value={position.id}>
                                                {position.positionName}
                                            </option>
                                        ))}
                                    </FormSelect>
                                </Col>
                            </Form.Group>

                            <Form.Group as={Row} controlId="formClubName">
                                <Form.Label column sm="3">Club Name</Form.Label>
                                <Col sm="9">
                                    <Form.Control
                                        type="text"
                                        placeholder="Club Name"
                                        value={editFormData.clubName}
                                        onChange={(e) => setEditFormData({ ...editFormData, clubName: e.target.value })}
                                        maxLength={30}
                                        required
                                    />
                                </Col>
                            </Form.Group>

                            <Form.Group as={Row} controlId="formLeague">
                                <Form.Label column sm="3">League</Form.Label>
                                <Col sm="9">
                                    <Form.Control
                                        type="text"
                                        placeholder="League"
                                        value={editFormData.league}
                                        onChange={(e) => setEditFormData({ ...editFormData, league: e.target.value })}
                                        maxLength={30}
                                        required
                                    />
                                </Col>
                            </Form.Group>

                            <Form.Group as={Row} controlId="formRegion">
                                <Form.Label column sm="3">Region</Form.Label>
                                <Col sm="9">
                                    <Form.Control
                                        type="text"
                                        placeholder="Region"
                                        value={editFormData.region}
                                        onChange={(e) => setEditFormData({ ...editFormData, region: e.target.value })}
                                        maxLength={30}
                                        required
                                    />
                                </Col>
                            </Form.Group>

                            <Form.Group as={Row} controlId="formStartDate">
                                <Form.Label column sm="3">Start Date</Form.Label>
                                <Col sm="9">
                                    <Form.Control
                                        type="date"
                                        value={TimeService.formatDateToForm(editFormData.startDate)}
                                        onChange={(e) => setEditFormData({ ...editFormData, startDate: e.target.value })}
                                        required
                                    />
                                </Col>
                            </Form.Group>

                            <Form.Group as={Row} controlId="formEndDate">
                                <Form.Label column sm="3">End Date</Form.Label>
                                <Col sm="9">
                                    <Form.Control
                                        type="date"
                                        value={TimeService.formatDateToForm(editFormData.endDate)}
                                        onChange={(e) => setEditFormData({ ...editFormData, endDate: e.target.value })}
                                        required
                                    />
                                </Col>
                            </Form.Group>

                            <Form.Group as={Row} controlId="formNumberOfMatches">
                                <Form.Label column sm="3">Matches</Form.Label>
                                <Col sm="9">
                                    <Form.Control
                                        type="number"
                                        placeholder="Number of Matches"
                                        value={editFormData.achievements.numberOfMatches}
                                        onChange={(e) => setEditFormData({
                                            ...editFormData,
                                            achievements: {
                                                ...editFormData.achievements,
                                                numberOfMatches: parseInt(e.target.value)
                                            }
                                        })}
                                        required
                                    />
                                </Col>
                            </Form.Group>

                            <Form.Group as={Row} controlId="formGoals">
                                <Form.Label column sm="3">Goals</Form.Label>
                                <Col sm="9">
                                    <Form.Control
                                        type="number"
                                        placeholder="Goals"
                                        value={editFormData.achievements.goals}
                                        onChange={(e) => setEditFormData({
                                            ...editFormData,
                                            achievements: {
                                                ...editFormData.achievements,
                                                goals: parseInt(e.target.value)
                                            }
                                        })}
                                        required
                                    />
                                </Col>
                            </Form.Group>

                            <Form.Group as={Row} controlId="formAssists">
                                <Form.Label column sm="3">Assists</Form.Label>
                                <Col sm="9">
                                    <Form.Control
                                        type="number"
                                        placeholder="Assists"
                                        value={editFormData.achievements.assists}
                                        onChange={(e) => setEditFormData({
                                            ...editFormData,
                                            achievements: {
                                                ...editFormData.achievements,
                                                assists: parseInt(e.target.value)
                                            }
                                        })}
                                        required
                                    />
                                </Col>
                            </Form.Group>

                            <Form.Group as={Row} controlId="formAdditionalAchievements">
                                <Form.Label column sm="3">Achievements</Form.Label>
                                <Col sm="9">
                                    <Form.Control
                                        as="textarea"
                                        placeholder="Additional Achievements"
                                        value={editFormData.achievements.additionalAchievements}
                                        onChange={(e) => setEditFormData({
                                            ...editFormData,
                                            achievements: {
                                                ...editFormData.achievements,
                                                additionalAchievements: e.target.value
                                            }
                                        })}
                                        maxLength={200}
                                    />
                                </Col>
                            </Form.Group>
                        </Form>
                    </Modal.Body>
                    <Modal.Footer>
                        <Button variant="secondary" onClick={() => setShowEditModal(false)}>Close</Button>
                        <Button variant="success" onClick={handleEditClubHistory}>Update</Button>
                    </Modal.Footer>
                </Modal>
            )}

            {/* Delete Club History Modal */}
            <Modal show={showDeleteModal} onHide={() => setShowDeleteModal(false)}>
                <Modal.Header closeButton>
                    <Modal.Title>Confirm action</Modal.Title>
                </Modal.Header>
                <Modal.Body>Are you sure you want to delete this club history?</Modal.Body>
                <Modal.Footer>
                    <Button variant="secondary" onClick={() => setShowDeleteModal(false)}>Cancel</Button>
                    <Button variant="danger" onClick={handleDeleteClubHistory}>Delete</Button>
                </Modal.Footer>
            </Modal>
        </div>
    );
}

export default ClubHistory;