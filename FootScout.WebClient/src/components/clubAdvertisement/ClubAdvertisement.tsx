import React, { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { Form, Button, Row, Col, Modal, FormSelect } from 'react-bootstrap';
import { toast } from 'react-toastify';
import AccountService from '../../services/api/AccountService';
import UserService from '../../services/api/UserService';
import TimeService from '../../services/time/TimeService';
import ClubAdvertisementService from '../../services/api/ClubAdvertisementService';
import FavoriteClubAdvertisementService from '../../services/api/FavoriteClubAdvertisementService';
import PlayerOfferService from '../../services/api/PlayerOfferService';
import OfferStatusService from '../../services/api/OfferStatusService';
import PlayerPositionService from '../../services/api/PlayerPositionService';
import PlayerFootService from '../../services/api/PlayerFootService';
import ChatService from '../../services/api/ChatService';
import UserDTO from '../../models/dtos/UserDTO';
import ClubAdvertisementModel from '../../models/interfaces/ClubAdvertisement';
import FavoriteClubAdvertisementCreateDTO from '../../models/dtos/FavoriteClubAdvertisementCreateDTO';
import PlayerOfferCreateDTO from '../../models/dtos/PlayerOfferCreateDTO';
import ChatCreateDTO from '../../models/dtos/ChatCreateDTO';
import PlayerPosition from '../../models/interfaces/PlayerPosition';
import PlayerFoot from '../../models/interfaces/PlayerFoot';
import OfferStatus from '../../models/interfaces/OfferStatus';
import '../../App.css';
import '../../styles/clubAdvertisement/ClubAdvertisement.css';

const ClubAdvertisement = () => {
    const { id } = useParams();
    const navigate = useNavigate();
    const [userId, setUserId] = useState<string | null>(null);
    const [clubAdvertisement, setClubAdvertisement] = useState<ClubAdvertisementModel | null>(null);
    const [clubMember, setClubMember] = useState<UserDTO | null>(null);
    const [clubAdvertisementStatus, setClubAdvertisementStatus] = useState<boolean | null>(null);
    const [favoriteId, setFavoriteId] = useState<number>(0);
    const [offerStatusId, setOfferStatusId] = useState<number>(0);
    const [positions, setPositions] = useState<PlayerPosition[]>([]);
    const [feet, setFeet] = useState<PlayerFoot[]>([]);
    const [offerStatuses, setOfferStatuses] = useState<OfferStatus[]>([]);
    const [isAdminRole, setIsAdminRole] = useState<boolean | null>(null);
    const [showEditModal, setShowEditModal] = useState<boolean>(false);
    const [showDeleteModal, setShowDeleteModal] = useState<boolean>(false);
    const [showFinishModal, setShowFinishModal] = useState<boolean>(false);
    const [showDeleteFavoriteModal, setShowDeleteFavoriteModal] = useState<boolean>(false);
    const [showSubmitPlayerOfferModal, setShowSubmitPlayerOfferModal] = useState<boolean>(false);
    const [editFormData, setEditFormData] = useState<ClubAdvertisementModel | null>(null);
    const [createFormData, setCreateFormData] = useState<PlayerOfferCreateDTO>({
        clubAdvertisementId: 0,
        playerPositionId: 0,
        age: 0,
        height: 0,
        playerFootId: 0,
        salary: 0,
        additionalInformation: '',
        playerId: ''
    });
    const [favoriteClubAdvertisementDTO, setFavoriteClubAdvertisementDTO] = useState<FavoriteClubAdvertisementCreateDTO>({
        clubAdvertisementId: 0,
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

        const fetchClubAdvertisementData = async (id: number) => {
            try {
                const clubAdvertisement = await ClubAdvertisementService.getClubAdvertisement(id);
                setClubAdvertisement(clubAdvertisement);

                const playerData = await UserService.getUser(clubAdvertisement.clubMemberId);
                setClubMember(playerData);

                const endDate = new Date(clubAdvertisement.endDate);
                const currentDate = new Date();
                setClubAdvertisementStatus(endDate >= currentDate);

                if (userId) {
                    const favoriteId = await FavoriteClubAdvertisementService.checkClubAdvertisementIsFavorite(clubAdvertisement.id, userId);
                    setFavoriteId(favoriteId);

                    const offferStatusId = await PlayerOfferService.getPlayerOfferStatusId(clubAdvertisement.id, userId);
                    setOfferStatusId(offferStatusId);
                }
            }
            catch (error) {
                console.error('Failed to fetch club advertisement:', error);
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
            fetchClubAdvertisementData(Number(id));
        }
    }, [id, userId]);

    if (!clubAdvertisement) {
        return <div><p><strong><h2>No club advertisement found...</h2></strong></p></div>;
    }

    const getOfferStatusNameById = (id: number) => {
        const offerStatus = offerStatuses.find(os => os.id === id);
        return offerStatus ? offerStatus.statusName : 'Unknown';
    };

    const handleShowEditModal = (clubAdvertisement: ClubAdvertisementModel) => {
        setEditFormData(clubAdvertisement);
        setShowEditModal(true);
    };

    const handleEditClubAdvertisement = async () => {
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

        try {
            const updatedFormData = {
                ...editFormData,
                playerPosition: position
            };

            await ClubAdvertisementService.updateClubAdvertisement(editFormData.id, updatedFormData);
            setShowEditModal(false);
            toast.success('Club advertisement updated successfully!');
            // Refresh the user data
            const _clubAdvertisement = await ClubAdvertisementService.getClubAdvertisement(clubAdvertisement.id);
            setClubAdvertisement(_clubAdvertisement);
        } catch (error) {
            console.error('Failed to update club advertisement:', error);
            toast.error('Failed to update club advertisement.');
        }
    };

    const validateAdvertisementForm = (formData: ClubAdvertisementModel) => {
        const { playerPositionId, clubName, league, region, salaryRange } = formData;
        const { min, max } = salaryRange;

        if (!playerPositionId || !clubName || !league || !region || !min || !max)
            return 'All fields are required.';

        if (isNaN(Number(min)) || isNaN(Number(max)))
            return 'Min and max salary must be numbers.';

        if (Number(min) < 0 || Number(max) < 0)
            return 'Min and max salary must be greater than or equal to 0.';

        if (max < min) {
            return 'Max Salary must be greater than Min Salary.';
        }

        return null;
    };

    const handleDeleteClubAdvertisement = async () => {
        if (!clubAdvertisement)
            return;

        try {
            await ClubAdvertisementService.deleteClubAdvertisement(clubAdvertisement.id);
            if(isAdminRole)
            {
                toast.success('Club advertisement has been deleted successfully.');
                navigate('/admin/club-advertisements');
            }
            else
            {
                toast.success('Your club advertisement has been deleted successfully.');
                navigate('/my-club-advertisements');
            }
        }
        catch (error) {
            console.error('Failed to delete club advertisement:', error);
            toast.error('Failed to delete club advertisement.');
        }
    };

    const handleFinishClubAdvertisement = async () => {
        if (!clubAdvertisement)
            return;

        const position = positions.find(pos => pos.id === clubAdvertisement.playerPositionId);
        if (!position) {
            toast.error('Invalid player position.');
            return;
        }

        try {
            const currentDate = new Date().toISOString();

            const updatedFormData = {
                ...clubAdvertisement,
                playerPosition: position,
                endDate: currentDate
            };

            await ClubAdvertisementService.updateClubAdvertisement(clubAdvertisement.id, updatedFormData);
            setShowFinishModal(false);
            if(isAdminRole)
            {
                toast.success('Club advertisement has been finished successfully.');
                navigate('/admin/club-advertisements');
            }
            else
            {
                toast.success('Your club advertisement has been finished successfully.');
                navigate('/my-club-advertisements');
            }
        }
        catch (error) {
            console.error('Failed to finish club advertisement:', error);
            toast.error('Failed to finish club advertisement.');
        }
    };

    const handleAddToFavorite = async () => {
        if (!clubAdvertisement || !userId)
            return;

        try {
            const createFormData = { ...favoriteClubAdvertisementDTO, clubAdvertisementId: clubAdvertisement.id, userId: userId };
            setFavoriteClubAdvertisementDTO(createFormData);

            await FavoriteClubAdvertisementService.addToFavorites(createFormData);
            toast.success('Club advertisement has been added to favorites successfully.');
            navigate('/my-favorite-club-advertisements');
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
            await FavoriteClubAdvertisementService.deleteFromFavorites(deleteFavoriteId);
            toast.success('Your followed advertisement has been deleted from favorites successfully.');
            setShowDeleteFavoriteModal(false);
            setDeleteFavoriteId(null);
            // Refresh the user data
            const favoriteId = await FavoriteClubAdvertisementService.checkClubAdvertisementIsFavorite(clubAdvertisement.id, userId);
            setFavoriteId(favoriteId);
        }
        catch (error) {
            console.error('Failed to delete advertisement from favorites:', error);
            toast.error('Failed to delete advertisement from favorites.');
        }
    };

    const handleSubmitPlayerOffer = async () => {
        if (!userId)
            return;

        const validationError = validateOfferForm(createFormData);
        if (validationError) {
            toast.error(validationError);
            return;
        }

        try {
            const newFormData = { ...createFormData, clubAdvertisementId: clubAdvertisement.id, playerId: userId };

            await PlayerOfferService.createPlayerOffer(newFormData);
            setShowSubmitPlayerOfferModal(false);
            toast.success('The player offer was submitted successfully.');
            navigate('/my-offers-as-player');
        }
        catch (error) {
            console.error('Failed to submit player offer:', error);
            toast.error('Failed to submit player offer.');
        }
    };

    const validateOfferForm = (formData: PlayerOfferCreateDTO) => {
        const { playerPositionId, age, height, playerFootId, salary } = formData;

        if (!playerPositionId || !age || !height || !playerFootId || !salary)
            return 'All fields are required.';

        if (isNaN(Number(age)) || isNaN(Number(height)) || isNaN(Number(salary)))
            return 'Age, height and salary must be numbers.';

        if (Number(age) < 0 || Number(height) < 0 || Number(salary) < 0)
            return 'Age, height and salary must be greater than or equal to 0.';

        return null;
    };

    const handleOpenChat = async () => {
        if (!clubAdvertisement || !userId) 
            return;

        try {
            let chatId = await ChatService.getChatIdBetweenUsers(userId, clubAdvertisement.clubMemberId);

            if (chatId === 0) {
                const chatCreateDTO: ChatCreateDTO = {
                    user1Id: userId,
                    user2Id: clubAdvertisement.clubMemberId
                };

                await ChatService.createChat(chatCreateDTO);
                chatId = await ChatService.getChatIdBetweenUsers(userId, clubAdvertisement.clubMemberId);
            }
            navigate(`/chat/${chatId}`, { state: { chatId } });
        } 
        catch (error) {
            console.error('Failed to open chat:', error);
            toast.error('Failed to open chat.');
        }
    };


    return (
        <div className="ClubAdvertisement">
            <h1><i className="bi bi-shield-fill"></i> Club Advertisement</h1>
            <div className="ad-buttons-container mb-3">
                {clubAdvertisementStatus ? (
                    clubAdvertisement.clubMemberId === userId || isAdminRole ? (
                        <Row>
                            <Col>
                                <Button variant="warning" className="ad-form-button" onClick={() => handleShowEditModal(clubAdvertisement)}>
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
                                    <Button variant="primary" className="ad-form-button" onClick={() => setShowSubmitPlayerOfferModal(true)}>
                                        <i className="bi bi-pen"></i> Submit a request
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
                        {(clubAdvertisement.clubMemberId === userId || isAdminRole) && (
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
                {clubAdvertisement && clubMember && (
                    <div>
                        <p><Form.Label className="ad-name-label">{(clubAdvertisement.clubName).toUpperCase()} - {(clubAdvertisement.playerPosition.positionName)}</Form.Label></p>
                        <Row>
                            <Col>
                                <Form.Label className="ad-section">CONTACT INFORMATION</Form.Label>
                                <p><Form.Label>Name: </Form.Label>
                                    <Form.Label className="ad-data-label">{clubMember.firstName} {clubMember.lastName}</Form.Label>
                                </p>
                                <p><Form.Label>E-mail: </Form.Label>
                                    <Form.Label className="ad-data-label">{clubMember.email}</Form.Label>
                                </p>
                                <p><Form.Label>Phone number: </Form.Label>
                                    <Form.Label className="ad-data-label">{clubMember.phoneNumber}</Form.Label>
                                </p>
                                <p><Form.Label>Location: </Form.Label>
                                    <Form.Label className="ad-data-label">{clubMember.location}</Form.Label>
                                </p>
                            </Col>
                            <Col>
                                <Form.Label className="ad-section">CLUB PREFERENCES</Form.Label>
                                <p><Form.Label>Position: </Form.Label>
                                    <Form.Label className="ad-data-label">{clubAdvertisement.playerPosition.positionName}</Form.Label>
                                </p>
                                <p><Form.Label>Club Name: </Form.Label>
                                    <Form.Label className="ad-data-label">{clubAdvertisement.clubName}</Form.Label>
                                </p>
                                <p><Form.Label>League (Region): </Form.Label>
                                    <Form.Label className="ad-data-label">{clubAdvertisement.league} ({clubAdvertisement.region})</Form.Label>
                                </p>
                                <p><Form.Label>Salary (zł.) / month: </Form.Label>
                                    <Form.Label className="ad-data-label">{clubAdvertisement.salaryRange.min} - {clubAdvertisement.salaryRange.max}</Form.Label>
                                </p>
                            </Col>
                        </Row>
                        <div>
                            <p><Form.Label>Creation Date: </Form.Label>
                                <Form.Label className="ad-creationDate-label">
                                    {TimeService.formatDateToEUR(clubAdvertisement.creationDate)}
                                </Form.Label>
                            </p>
                            {clubAdvertisementStatus ? (
                                <p><Form.Label>End Date (days left): </Form.Label>
                                    <Form.Label className="ad-creationDate-label">
                                        {TimeService.formatDateToEUR(clubAdvertisement.endDate)} ({TimeService.calculateDaysLeft(clubAdvertisement.endDate)} days)
                                    </Form.Label>
                                </p>
                            ) : (
                                <p><Form.Label>End Date (days ago): </Form.Label>
                                    <Form.Label className="ad-creationDate-label">
                                        {TimeService.formatDateToEUR(clubAdvertisement.endDate)} ({TimeService.calculateSkippedDays(clubAdvertisement.endDate)} days)
                                    </Form.Label>
                                </p>
                            )}
                        </div>
                    </div>
                )}
            </div>

            {/* Edit ClubAdvertisement Modal */}
            {editFormData && (
                <Modal show={showEditModal} onHide={() => setShowEditModal(false)}>
                    <Modal.Header closeButton>
                        <Modal.Title>Edit Club History</Modal.Title>
                    </Modal.Header>
                    <Modal.Body>
                        <Form>
                            <Row>
                                <Form.Group className="mb-3" controlId="formClubName">
                                    <Form.Label>Club Name</Form.Label>
                                    <Form.Control
                                        type="text"
                                        placeholder="Club Name"
                                        value={editFormData.clubName}
                                        onChange={(e) => setEditFormData({ ...editFormData, clubName: e.target.value })}
                                        maxLength={30}
                                        required
                                    />
                                </Form.Group>
                            </Row>
                            <Row>
                                <Form.Group className="mb-3" controlId="formLeague">
                                    <Form.Label>League</Form.Label>
                                    <Form.Control
                                        type="text"
                                        placeholder="League"
                                        value={editFormData.league}
                                        onChange={(e) => setEditFormData({ ...editFormData, league: e.target.value })}
                                        maxLength={30}
                                        required
                                    />
                                </Form.Group>
                            </Row>
                            <Row>
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
                            </Row>
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
                        <Button variant="success" onClick={handleEditClubAdvertisement}>Update</Button>
                    </Modal.Footer>
                </Modal>
            )}

            {/* Delete Club Advertisement */}
            <Modal show={showDeleteModal} onHide={() => setShowDeleteModal(false)}>
                <Modal.Header closeButton>
                    <Modal.Title>Confirm action</Modal.Title>
                </Modal.Header>
                <Modal.Body>Are you sure you want to delete this advertisement?</Modal.Body>
                <Modal.Footer>
                    <Button variant="secondary" onClick={() => setShowDeleteModal(false)}>Cancel</Button>
                    <Button variant="danger" onClick={handleDeleteClubAdvertisement}>Delete</Button>
                </Modal.Footer>
            </Modal>

            {/* Finish Club Advertisement */}
            <Modal show={showFinishModal} onHide={() => setShowFinishModal(false)}>
                <Modal.Header closeButton>
                    <Modal.Title>Confirm action</Modal.Title>
                </Modal.Header>
                <Modal.Body>Are you sure you want to finish this advertisement?</Modal.Body>
                <Modal.Footer>
                    <Button variant="secondary" onClick={() => setShowFinishModal(false)}>Cancel</Button>
                    <Button variant="dark" onClick={handleFinishClubAdvertisement}>Finish</Button>
                </Modal.Footer>
            </Modal>

            {/* Delete Favorite Club Advertisement Modal */}
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

            {/* Create Submit Player Offer Modal */}
            <Modal show={showSubmitPlayerOfferModal} onHide={() => setShowSubmitPlayerOfferModal(false)}>
                <Modal.Header closeButton>
                    <Modal.Title>Submit player request</Modal.Title>
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
                        <Form.Group as={Row} controlId="formAge">
                            <Form.Label column sm="3">Age</Form.Label>
                            <Col sm="9">
                                <Form.Control
                                    type="number"
                                    placeholder="Age"
                                    value={createFormData.age}
                                    onChange={(e) => setCreateFormData({ ...createFormData, age: parseInt(e.target.value) })}
                                    required
                                />
                            </Col>
                        </Form.Group>
                        <Form.Group as={Row} controlId="formHeight">
                            <Form.Label column sm="3">Height</Form.Label>
                            <Col sm="9">
                                <Form.Control
                                    type="number"
                                    placeholder="Height"
                                    value={createFormData.height}
                                    onChange={(e) => setCreateFormData({ ...createFormData, height: parseInt(e.target.value) })}
                                    required
                                />
                            </Col>
                        </Form.Group>
                        <Form.Group as={Row} controlId="formFoot">
                            <Form.Label column sm="3">Foot</Form.Label>
                            <Col sm="9">
                                <FormSelect
                                    value={createFormData.playerFootId}
                                    onChange={(e) => setCreateFormData({
                                        ...createFormData,
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
                    <Button variant="secondary" onClick={() => setShowSubmitPlayerOfferModal(false)}>Close</Button>
                    <Button variant="success" onClick={handleSubmitPlayerOffer}>Submit</Button>
                </Modal.Footer>
            </Modal>
        </div >
    );
}

export default ClubAdvertisement;