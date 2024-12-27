import React, { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { Table, Form, Button, Row, Col, Modal, FormSelect } from 'react-bootstrap';
import { toast } from 'react-toastify';
import AccountService from '../../services/api/AccountService';
import UserService from '../../services/api/UserService';
import TimeService from '../../services/time/TimeService';
import PlayerAdvertisementService from '../../services/api/PlayerAdvertisementService';
import FavoritePlayerAdvertisementService from '../../services/api/FavoritePlayerAdvertisementService';
import ClubOfferService from '../../services/api/ClubOfferService';
import OfferStatusService from '../../services/api/OfferStatusService';
import PlayerPositionService from '../../services/api/PlayerPositionService';
import PlayerFootService from '../../services/api/PlayerFootService';
import ChatService from '../../services/api/ChatService';
import UserDTO from '../../models/dtos/UserDTO';
import ClubHistoryModel from '../../models/interfaces/ClubHistory';
import PlayerAdvertisementModel from '../../models/interfaces/PlayerAdvertisement';
import FavoritePlayerAdvertisementCreateDTO from '../../models/dtos/FavoritePlayerAdvertisementCreateDTO';
import ClubOfferCreateDTO from '../../models/dtos/ClubOfferCreateDTO';
import ChatCreateDTO from '../../models/dtos/ChatCreateDTO';
import PlayerPosition from '../../models/interfaces/PlayerPosition';
import PlayerFoot from '../../models/interfaces/PlayerFoot';
import OfferStatus from '../../models/interfaces/OfferStatus';
import '../../App.css';
import '../../styles/playerAdvertisement/PlayerAdvertisement.css';

const PlayerAdvertisement = () => {
    const { id } = useParams();
    const navigate = useNavigate();
    const [userId, setUserId] = useState<string | null>(null);
    const [playerAdvertisement, setPlayerAdvertisement] = useState<PlayerAdvertisementModel | null>(null);
    const [player, setPlayer] = useState<UserDTO | null>(null);
    const [playerClubHistories, setPlayerClubHistories] = useState<ClubHistoryModel[]>([]);
    const [playerAdvertisementStatus, setPlayerAdvertisementStatus] = useState<boolean | null>(null);
    const [favoriteId, setFavoriteId] = useState<number>(0);
    const [offerStatusId, setOfferStatusId] = useState<number>(0);
    const [positions, setPositions] = useState<PlayerPosition[]>([]);
    const [feet, setFeet] = useState<PlayerFoot[]>([]);
    const [offerStatuses, setOfferStatuses] = useState<OfferStatus[]>([]);
    const [isAdminRole, setIsAdminRole] = useState<boolean | null>(null);
    const [showClubHistoryDetailsModal, setShowClubHistoryDetailsModal] = useState<boolean>(false);
    const [showEditModal, setShowEditModal] = useState<boolean>(false);
    const [showDeleteModal, setShowDeleteModal] = useState<boolean>(false);
    const [showFinishModal, setShowFinishModal] = useState<boolean>(false);
    const [showDeleteFavoriteModal, setShowDeleteFavoriteModal] = useState<boolean>(false);
    const [showSubmitClubOfferModal, setShowSubmitClubOfferModal] = useState<boolean>(false);
    const [selectedClubHistory, setSelectedClubHistory] = useState<ClubHistoryModel | null>(null);
    const [editFormData, setEditFormData] = useState<PlayerAdvertisementModel | null>(null);
    const [createFormData, setCreateFormData] = useState<ClubOfferCreateDTO>({
        playerAdvertisementId: 0,
        playerPositionId: 0,
        clubName: '',
        league: '',
        region: '',
        salary: 0,
        additionalInformation: '',
        clubMemberId: ''
    });
    const [favoritePlayerAdvertisementDTO, setFavoritePlayerAdvertisementDTO] = useState<FavoritePlayerAdvertisementCreateDTO>({
        playerAdvertisementId: 0,
        userId: ''
    });
    const [deleteFavoriteId, setDeleteFavoriteId] = useState<number | null>(null);

    useEffect(() => {
        const fetchUserData = async () => {
            try {
                const userId = await AccountService.getId();
                if (userId)
                    setUserId(userId);

                const isAdmin = await AccountService.isRoleAdmin();
                setIsAdminRole(isAdmin);
            }
            catch (error) {
                console.error('Failed to fetch userId:', error);
                toast.error('Failed to load userId.');
            }
        };

        const fetchPlayerAdvertisementData = async (id: number) => {
            try {
                const playerAdvertisement = await PlayerAdvertisementService.getPlayerAdvertisement(id);
                setPlayerAdvertisement(playerAdvertisement);

                const playerData = await UserService.getUser(playerAdvertisement.playerId);
                setPlayer(playerData);

                const playerClubHistories = await UserService.getUserClubHistory(playerAdvertisement.playerId);
                setPlayerClubHistories(playerClubHistories);

                const endDate = new Date(playerAdvertisement.endDate);
                const currentDate = new Date();
                setPlayerAdvertisementStatus(endDate >= currentDate);

                if (userId) {
                    const favoriteId = await FavoritePlayerAdvertisementService.checkPlayerAdvertisementIsFavorite(playerAdvertisement.id, userId);
                    setFavoriteId(favoriteId);

                    const offferStatusId = await ClubOfferService.getClubOfferStatusId(playerAdvertisement.id, userId);
                    setOfferStatusId(offferStatusId);
                }
            }
            catch (error) {
                console.error('Failed to fetch player advertisement:', error);
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

        const fetchFeet = async () => {
            try {
                const feetData = await PlayerFootService.getPlayerFeet();
                setFeet(feetData);
            }
            catch (error) {
                console.error('Failed to fetch foot names:', error);
                toast.error('Failed to load foot names.');
            }
        };

        const fetchOfferStatuses = async () => {
            try {
                const offerStatuses = await OfferStatusService.getOfferStatuses();
                setOfferStatuses(offerStatuses);
            }
            catch (error) {
                console.error('Failed to fetch offer statuses:', error);
                toast.error('Failed to load offer statuses.');
            }
        };

        if (id) {
            fetchUserData();
            fetchPositions();
            fetchFeet();
            fetchOfferStatuses();
            fetchPlayerAdvertisementData(Number(id));
        }
    }, [id, userId]);

    if (!playerAdvertisement) {
        return <div><p><strong><h2>No player advertisement found...</h2></strong></p></div>;
    }

    const getOfferStatusNameById = (id: number) => {
        const offerStatus = offerStatuses.find(os => os.id === id);
        return offerStatus ? offerStatus.statusName : 'Unknown';
    };

    const handleShowClubHistoryDetails = (clubHistory: ClubHistoryModel) => {
        setSelectedClubHistory(clubHistory);
        setShowClubHistoryDetailsModal(true);
    };

    const handleShowEditModal = (playerAdvertisement: PlayerAdvertisementModel) => {
        setEditFormData(playerAdvertisement);
        setShowEditModal(true);
    };

    const handleEditPlayerAdvertisement = async () => {
        if (!editFormData)
            return;

        const validationError = validateAdvertisementForm(editFormData);
        if (validationError) {
            toast.error(validationError);
            return;
        }

        const position = positions.find(pos => pos.id === editFormData.playerPositionId);
        if (!position) {
            toast.error('Invalid player position.');
            return;
        }

        const foot = feet.find(f => f.id === editFormData.playerFootId);
        if (!foot) {
            toast.error('Invalid foot name.');
            return;
        }

        try {
            const updatedFormData = {
                ...editFormData,
                playerPosition: position,
                playerFoot: foot
            };

            await PlayerAdvertisementService.updatePlayerAdvertisement(editFormData.id, updatedFormData);
            setShowEditModal(false);
            toast.success('Player advertisement updated successfully!');
            // Refresh the user data
            const _playerAdvertisement = await PlayerAdvertisementService.getPlayerAdvertisement(playerAdvertisement.id);
            setPlayerAdvertisement(_playerAdvertisement);
        } catch (error) {
            console.error('Failed to update player advertisement:', error);
            toast.error('Failed to update player advertisement.');
        }
    };

    const validateAdvertisementForm = (formData: PlayerAdvertisementModel) => {
        const { playerPositionId, league, region, age, height, playerFootId, salaryRange } = formData;
        const { min, max } = salaryRange;

        if (!playerPositionId || !league || !region || !age || !height || !playerFootId || !min || !max)
            return 'All fields are required.';

        if (isNaN(Number(age)) || isNaN(Number(height)) || isNaN(Number(min)) || isNaN(Number(max)))
            return 'Age, height, min and max salary must be numbers.';

        if (Number(age) < 0 || Number(height) < 0 || Number(min) < 0 || Number(max) < 0)
            return 'Age, height, min and max salary must be greater than or equal to 0.';

        if (max < min) {
            return 'Max Salary must be greater than Min Salary.';
        }

        return null;
    };

    const handleDeletePlayerAdvertisement = async () => {
        if (!playerAdvertisement)
            return;

        try {
            await PlayerAdvertisementService.deletePlayerAdvertisement(playerAdvertisement.id);
            if(isAdminRole)
            {
                toast.success('Player advertisement has been deleted successfully.');
                navigate('/admin/player-advertisements');
            }
            else
            {
                toast.success('Your player advertisement has been deleted successfully.');
                navigate('/my-player-advertisements');
            }
        }
        catch (error) {
            console.error('Failed to delete player advertisement:', error);
            toast.error('Failed to delete player advertisement.');
        }
    };

    const handleFinishPlayerAdvertisement = async () => {
        if (!playerAdvertisement)
            return;

        const position = positions.find(pos => pos.id === playerAdvertisement.playerPositionId);
        if (!position) {
            toast.error('Invalid player position.');
            return;
        }

        const foot = feet.find(f => f.id === playerAdvertisement.playerFootId);
        if (!foot) {
            toast.error('Invalid foot name.');
            return;
        }

        try {
            const currentDate = new Date().toISOString();

            const updatedFormData = {
                ...playerAdvertisement,
                playerPosition: position,
                playerFoot: foot,
                endDate: currentDate
            };

            await PlayerAdvertisementService.updatePlayerAdvertisement(playerAdvertisement.id, updatedFormData);
            setShowFinishModal(false);
            if(isAdminRole)
            {
                toast.success('Player advertisement has been finished successfully.');
                navigate('/admin/player-advertisements');
            }
            else
            {   
                toast.success('Your player advertisement has been finished successfully.');
                navigate('/my-player-advertisements');
            }
        }
        catch (error) {
            console.error('Failed to finish player advertisement:', error);
            toast.error('Failed to finish player advertisement.');
        }
    };

    const handleAddToFavorite = async () => {
        if (!playerAdvertisement || !userId)
            return;

        try {
            const createFormData = { ...favoritePlayerAdvertisementDTO, playerAdvertisementId: playerAdvertisement.id, userId: userId };
            setFavoritePlayerAdvertisementDTO(createFormData);

            await FavoritePlayerAdvertisementService.addToFavorites(createFormData);
            toast.success('Player advertisement has been added to favorites successfully.');
            navigate('/my-favorite-player-advertisements');
        }
        catch (error) {
            console.error('Failed to add advertisement to favorites:', error);
            toast.error('Failed to add advertisement to favorites.');
        }
    };

    const handleShowDeleteFavoriteModal = (favoriteAdvertisementId: number) => {
        setDeleteFavoriteId(favoriteAdvertisementId);
        setShowDeleteFavoriteModal(true);
    };

    const handleDeleteFromFavorites = async () => {
        if (!userId || !deleteFavoriteId)
            return;

        try {
            await FavoritePlayerAdvertisementService.deleteFromFavorites(deleteFavoriteId);
            toast.success('Your followed advertisement has been deleted from favorites successfully.');
            setShowDeleteFavoriteModal(false);
            setDeleteFavoriteId(null);
            // Refresh the user data
            const favoriteId = await FavoritePlayerAdvertisementService.checkPlayerAdvertisementIsFavorite(playerAdvertisement.id, userId);
            setFavoriteId(favoriteId);
        }
        catch (error) {
            console.error('Failed to delete advertisement from favorites:', error);
            toast.error('Failed to delete advertisement from favorites.');
        }
    };

    const handleSubmitClubOffer = async () => {
        if (!userId)
            return;

        const validationError = validateOfferForm(createFormData);
        if (validationError) {
            toast.error(validationError);
            return;
        }

        try {
            const newFormData = { ...createFormData, playerAdvertisementId: playerAdvertisement.id, clubMemberId: userId };

            await ClubOfferService.createClubOffer(newFormData);
            setShowSubmitClubOfferModal(false);
            toast.success('The club offer was submitted successfully.');
            navigate('/my-offers-as-club');
        }
        catch (error) {
            console.error('Failed to submit club offer:', error);
            toast.error('Failed to submit club offer.');
        }
    };

    const validateOfferForm = (formData: ClubOfferCreateDTO) => {
        const { playerPositionId, league, region, salary } = formData;

        if (!playerPositionId || !league || !region || !salary)
            return 'All fields are required.';

        if (isNaN(Number(salary)))
            return 'salary must be numbers.';

        if (Number(salary) < 0)
            return 'salary must be greater than or equal to 0.';

        return null;
    };

    const handleOpenChat = async () => {
        if (!playerAdvertisement || !userId) 
            return;

        try {
            let chatId = await ChatService.getChatIdBetweenUsers(userId, playerAdvertisement.playerId);

            if (chatId === 0) {
                const chatCreateDTO: ChatCreateDTO = {
                    user1Id: userId,
                    user2Id: playerAdvertisement.playerId
                };

                await ChatService.createChat(chatCreateDTO);
                chatId = await ChatService.getChatIdBetweenUsers(userId, playerAdvertisement.playerId);
            }
            navigate(`/chat/${chatId}`, { state: { chatId } });
        } 
        catch (error) {
            console.error('Failed to open chat:', error);
            toast.error('Failed to open chat.');
        }
    };

    return (
        <div className="PlayerAdvertisement">
            <h1><i className="bi bi-person-bounding-box"></i> Player Advertisement</h1>
            <div className="ad-buttons-container mb-3">
                {playerAdvertisementStatus ? (
                    playerAdvertisement.playerId === userId || isAdminRole ? (
                        <Row>
                            <Col>
                                <Button variant="warning" className="ad-form-button" onClick={() => handleShowEditModal(playerAdvertisement)}>
                                    <i className="bi bi-pencil-square"></i> Edit
                                </Button>
                            </Col>
                            <Col>
                                <Button variant="secondary" className="ad-form-button" onClick={() => setShowFinishModal(true)}>
                                    <i className="bi bi-calendar-x"></i> Finish ad
                                </Button>
                            </Col>
                            <Col>
                                <Button variant="danger" className="ad-form-button" onClick={() => setShowDeleteModal(true)}>
                                    <i className="bi bi-trash"></i> Delete
                                </Button>
                            </Col>
                            {(isAdminRole) && (
                                <Col>
                                    <Button variant="info" className="ad-form-button" onClick={handleOpenChat}>
                                        <i className="bi bi-chat-fill"></i> Chat
                                    </Button>
                                </Col>
                            )}
                        </Row>
                    ) : (
                        <Row>
                            {offerStatusId === 0 ? (
                                <Col>
                                    <Button variant="primary" className="ad-form-button" onClick={() => setShowSubmitClubOfferModal(true)}>
                                        <i className="bi bi-pen"></i> Submit an offer
                                    </Button>
                                </Col>
                            ) : (
                                <Col>
                                    <p>
                                        <Form.Label className="status-label">
                                            {getOfferStatusNameById(offerStatusId) === "Offered" && (
                                                <i className="bi bi-question-diamond-fill" style={{ color: '#b571ff' }}></i>
                                            )}
                                            {getOfferStatusNameById(offerStatusId) === "Accepted" && (
                                                <i className="bi bi-check-circle-fill" style={{ color: '#74ee54' }}></i>
                                            )}
                                            {getOfferStatusNameById(offerStatusId) === "Rejected" && (
                                                <i className="bi bi-x-circle-fill" style={{ color: '#e33b3b' }}></i>
                                            )}
                                            {getOfferStatusNameById(offerStatusId)} Offer
                                        </Form.Label>
                                    </p>
                                </Col>
                            )}

                            {favoriteId === 0 ? (
                                <Col>
                                    <Button variant="success" onClick={handleAddToFavorite}>
                                        <i className="bi bi-heart"></i>
                                    </Button>
                                </Col>
                            ) : (
                                <Col>
                                    <Button variant="danger" onClick={() => handleShowDeleteFavoriteModal(favoriteId)}>
                                        <i className="bi bi-heart-fill"></i>
                                    </Button>
                                </Col>
                            )}

                            <Col>
                                <Button variant="info" onClick={handleOpenChat}>
                                    <i className="bi bi-chat-fill"></i>
                                </Button>
                            </Col>
                        </Row>
                    )
                ) : (
                    <div className="ad-status-container">
                        <p>Status: <strong>Inactive</strong></p>
                        {(playerAdvertisement.playerId === userId || isAdminRole) && (
                            <Row>
                                <Col>
                                    <Button variant="danger" onClick={() => setShowDeleteModal(true)}>
                                        <i className="bi bi-trash"></i> Delete
                                    </Button>
                                </Col>
                            </Row>
                        )}
                    </div>
                )}
            </div>
            <div className="ad-container">
                {playerAdvertisement && player && (
                    <div>
                        <p><Form.Label className="ad-name-label">{(player.firstName).toUpperCase()} {(player.lastName).toUpperCase()} - {playerAdvertisement.playerPosition.positionName}</Form.Label></p>
                        <Row>
                            <Col>
                                <Form.Label className="ad-section">CONTACT INFORMATION</Form.Label>
                                <p><Form.Label>E-mail: </Form.Label>
                                    <Form.Label className="ad-data-label">{player.email}</Form.Label>
                                </p>
                                <p><Form.Label>Phone number: </Form.Label>
                                    <Form.Label className="ad-data-label">{player.phoneNumber}</Form.Label>
                                </p>
                                <p><Form.Label>Location: </Form.Label>
                                    <Form.Label className="ad-data-label">{player.location}</Form.Label>
                                </p>
                            </Col>
                            <Col>
                                <Form.Label className="ad-section">PLAYER PROFILE</Form.Label>
                                <p><Form.Label>Age: </Form.Label>
                                    <Form.Label className="ad-data-label">{playerAdvertisement.age}</Form.Label>
                                </p>
                                <p><Form.Label>Height: </Form.Label>
                                    <Form.Label className="ad-data-label">{playerAdvertisement.height}</Form.Label>
                                </p>
                                <p><Form.Label>Foot: </Form.Label>
                                    <Form.Label className="ad-data-label">{playerAdvertisement.playerFoot.footName}</Form.Label>
                                </p>
                            </Col>
                            <Col>
                                <Form.Label className="ad-section">PREFERENCES</Form.Label>
                                <p><Form.Label>Position: </Form.Label>
                                    <Form.Label className="ad-data-label">{playerAdvertisement.playerPosition.positionName}</Form.Label>
                                </p>
                                <p><Form.Label>League (Region): </Form.Label>
                                    <Form.Label className="ad-data-label">{playerAdvertisement.league} ({playerAdvertisement.region})</Form.Label>
                                </p>
                                <p><Form.Label>Salary (zł.) / month: </Form.Label>
                                    <Form.Label className="ad-data-label">{playerAdvertisement.salaryRange.min} - {playerAdvertisement.salaryRange.max}</Form.Label>
                                </p>
                            </Col>
                        </Row>
                        <Form.Label className="ad-section">CLUB HISTORY</Form.Label>
                        <div className="ad-table-responsive">
                            <Table striped bordered hover variant="light">
                                <thead className="table-dark">
                                    <tr>
                                        <th>Date</th>
                                        <th>Club</th>
                                        <th>League (Region)</th>
                                        <th>Position</th>
                                        <th></th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {playerClubHistories.length > 0 ? (
                                        playerClubHistories.map((history, index) => (
                                            <tr key={index}>
                                                <td>{TimeService.formatDateToEUR(history.startDate)} - {TimeService.formatDateToEUR(history.endDate)}</td>
                                                <td>{history.clubName}</td>
                                                <td>{history.league} ({history.region})</td>
                                                <td>{history.playerPosition.positionName}</td>
                                                <td>
                                                    <Button variant="dark" className="button-spacing" onClick={() => handleShowClubHistoryDetails(history)}>
                                                        <i className="bi bi-info-square"></i>
                                                    </Button>
                                                </td>
                                            </tr>
                                        ))
                                    ) : (
                                        <tr>
                                            <td colSpan={5} className="text-center">No club history available</td>
                                        </tr>
                                    )}
                                </tbody>
                            </Table>
                            <p><Form.Label>Creation Date: </Form.Label>
                                <Form.Label className="ad-creationDate-label">
                                    {TimeService.formatDateToEUR(playerAdvertisement.creationDate)}
                                </Form.Label>
                            </p>
                            {playerAdvertisementStatus ? (
                                <p><Form.Label>End Date (days left): </Form.Label>
                                    <Form.Label className="ad-creationDate-label">
                                        {TimeService.formatDateToEUR(playerAdvertisement.endDate)} ({TimeService.calculateDaysLeft(playerAdvertisement.endDate)} days)
                                    </Form.Label>
                                </p>
                            ) : (
                                <p><Form.Label>End Date (days ago): </Form.Label>
                                    <Form.Label className="ad-creationDate-label">
                                        {TimeService.formatDateToEUR(playerAdvertisement.endDate)} ({TimeService.calculateSkippedDays(playerAdvertisement.endDate)} days)
                                    </Form.Label>
                                </p>
                            )}
                        </div>
                    </div>
                )}
            </div>

            {/* Details of Club History Modal */}
            <Modal show={showClubHistoryDetailsModal} onHide={() => setShowClubHistoryDetailsModal(false)}>
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
                    <Button variant="secondary" onClick={() => setShowClubHistoryDetailsModal(false)}>Close</Button>
                </Modal.Footer>
            </Modal>

            {/* Edit PlayerAdvertisement Modal */}
            {editFormData && (
                <Modal show={showEditModal} onHide={() => setShowEditModal(false)}>
                    <Modal.Header closeButton>
                        <Modal.Title>Edit Club History</Modal.Title>
                    </Modal.Header>
                    <Modal.Body>
                        <Form>
                            <Form.Group className="mb-3" controlId="formPosition">
                                <Form.Label>Position</Form.Label>
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
                            </Form.Group>
                            <Row>
                                <Col>
                                    <Form.Group className="mb-3" controlId="formLeague">
                                        <Form.Label>Preferred League</Form.Label>
                                        <Form.Control
                                            type="text"
                                            placeholder="League"
                                            value={editFormData.league}
                                            onChange={(e) => setEditFormData({ ...editFormData, league: e.target.value })}
                                            maxLength={30}
                                            required
                                        />
                                    </Form.Group>
                                </Col>
                                <Col>
                                    <Form.Group className="mb-3" controlId="formRegion">
                                        <Form.Label>Region</Form.Label>
                                        <Form.Control
                                            type="text"
                                            placeholder="Region"
                                            value={editFormData.region}
                                            onChange={(e) => setEditFormData({ ...editFormData, region: e.target.value })}
                                            maxLength={30}
                                            required
                                        />
                                    </Form.Group>
                                </Col>
                            </Row>
                            <Row>
                                <Col>
                                    <Form.Group className="mb-3" controlId="formAge">
                                        <Form.Label>Age</Form.Label>
                                        <Form.Control
                                            type="number"
                                            placeholder="Age"
                                            value={editFormData.age}
                                            onChange={(e) => setEditFormData({ ...editFormData, age: parseInt(e.target.value) })}
                                            required
                                        />
                                    </Form.Group>
                                </Col>
                                <Col>
                                    <Form.Group className="mb-3" controlId="formHeight">
                                        <Form.Label>Height (cm)</Form.Label>
                                        <Form.Control
                                            type="number"
                                            placeholder="Height"
                                            value={editFormData.height}
                                            onChange={(e) => setEditFormData({ ...editFormData, height: parseInt(e.target.value) })}
                                            required
                                        />
                                    </Form.Group>
                                </Col>
                            </Row>
                            <Form.Group className="mb-3" controlId="formFoot">
                                <Form.Label>Foot</Form.Label>
                                <FormSelect
                                    value={editFormData.playerFootId}
                                    onChange={(e) => setEditFormData({
                                        ...editFormData,
                                        playerFootId: parseInt(e.target.value, 10)
                                    })}
                                >
                                    <option value="">Select Foot</option>
                                    {feet.map((foot) => (
                                        <option key={foot.id} value={foot.id}>
                                            {foot.footName}
                                        </option>
                                    ))}
                                </FormSelect>
                            </Form.Group>
                            <Row>
                                <Col>
                                    <Form.Group className="mb-3" controlId="formMin">
                                        <Form.Label>Min Salary (zł.) / month</Form.Label>
                                        <Form.Control
                                            type="number"
                                            placeholder="Min"
                                            value={editFormData.salaryRange.min}
                                            onChange={(e) => setEditFormData({
                                                ...editFormData,
                                                salaryRange: {
                                                    ...editFormData.salaryRange,
                                                    min: parseFloat(e.target.value)
                                                }
                                            })}
                                            required
                                        />
                                    </Form.Group>
                                </Col>
                                <Col>
                                    <Form.Group className="mb-3" controlId="formMax">
                                        <Form.Label>Max Salary (zł.) / month</Form.Label>
                                        <Form.Control
                                            type="number"
                                            placeholder="Max"
                                            value={editFormData.salaryRange.max}
                                            onChange={(e) => setEditFormData({
                                                ...editFormData,
                                                salaryRange: {
                                                    ...editFormData.salaryRange,
                                                    max: parseFloat(e.target.value)
                                                }
                                            })}
                                            required
                                        />
                                    </Form.Group>
                                </Col>
                            </Row>
                        </Form>
                    </Modal.Body>
                    <Modal.Footer>
                        <Button variant="secondary" onClick={() => setShowEditModal(false)}>Close</Button>
                        <Button variant="success" onClick={handleEditPlayerAdvertisement}>Update</Button>
                    </Modal.Footer>
                </Modal>
            )
            }

            {/* Delete Player Advertisement */}
            <Modal show={showDeleteModal} onHide={() => setShowDeleteModal(false)}>
                <Modal.Header closeButton>
                    <Modal.Title>Confirm action</Modal.Title>
                </Modal.Header>
                <Modal.Body>Are you sure you want to delete this advertisement?</Modal.Body>
                <Modal.Footer>
                    <Button variant="secondary" onClick={() => setShowDeleteModal(false)}>Cancel</Button>
                    <Button variant="danger" onClick={handleDeletePlayerAdvertisement}>Delete</Button>
                </Modal.Footer>
            </Modal>

            {/* Finish Player Advertisement */}
            <Modal show={showFinishModal} onHide={() => setShowFinishModal(false)}>
                <Modal.Header closeButton>
                    <Modal.Title>Confirm action</Modal.Title>
                </Modal.Header>
                <Modal.Body>Are you sure you want to finish this advertisement?</Modal.Body>
                <Modal.Footer>
                    <Button variant="secondary" onClick={() => setShowFinishModal(false)}>Cancel</Button>
                    <Button variant="dark" onClick={handleFinishPlayerAdvertisement}>Finish</Button>
                </Modal.Footer>
            </Modal>

            {/* Delete Favorite Player Advertisement Modal */}
            <Modal show={showDeleteFavoriteModal} onHide={() => setShowDeleteFavoriteModal(false)}>
                <Modal.Header closeButton>
                    <Modal.Title>Confirm action</Modal.Title>
                </Modal.Header>
                <Modal.Body>Are you sure you want to delete this advertisement from favorites?</Modal.Body>
                <Modal.Footer>
                    <Button variant="secondary" onClick={() => setShowDeleteFavoriteModal(false)}>Cancel</Button>
                    <Button variant="danger" onClick={handleDeleteFromFavorites}>Delete</Button>
                </Modal.Footer>
            </Modal>

            {/* Create Submit Club Offer Modal */}
            <Modal show={showSubmitClubOfferModal} onHide={() => setShowSubmitClubOfferModal(false)}>
                <Modal.Header closeButton>
                    <Modal.Title>Submit club offer</Modal.Title>
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
                            <Form.Label column sm="3">Club name</Form.Label>
                            <Col sm="9">
                                <Form.Control
                                    type="text"
                                    placeholder="ClubName"
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

                        <Form.Group as={Row} controlId="formSalary">
                            <Form.Label column sm="3">Salary (zł.) / month</Form.Label>
                            <Col sm="9">
                                <Form.Control
                                    type="number"
                                    placeholder="Salary"
                                    value={createFormData.salary}
                                    onChange={(e) => setCreateFormData({ ...createFormData, salary: parseFloat(e.target.value) })}
                                    required
                                />
                            </Col>
                        </Form.Group>

                        <Form.Group as={Row} controlId="formAdditionalInformation">
                            <Form.Label column sm="3">Additional Information</Form.Label>
                            <Col sm="9">
                                <Form.Control
                                    as="textarea"
                                    placeholder="Additional Information"
                                    value={createFormData.additionalInformation}
                                    onChange={(e) => setCreateFormData({ ...createFormData, additionalInformation: e.target.value })}
                                    maxLength={200}
                                />
                            </Col>
                        </Form.Group>
                    </Form>
                </Modal.Body>
                <Modal.Footer>
                    <Button variant="secondary" onClick={() => setShowSubmitClubOfferModal(false)}>Close</Button>
                    <Button variant="success" onClick={handleSubmitClubOffer}>Submit</Button>
                </Modal.Footer>
            </Modal>
        </div >
    );
}

export default PlayerAdvertisement;