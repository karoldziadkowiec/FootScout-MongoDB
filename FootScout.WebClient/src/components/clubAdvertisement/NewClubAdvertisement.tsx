import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { Form, Button, Col, Row, Container, FormSelect } from 'react-bootstrap';
import { toast } from 'react-toastify';
import AccountService from '../../services/api/AccountService';
import ClubAdvertisementService from '../../services/api/ClubAdvertisementService';
import PlayerPositionService from '../../services/api/PlayerPositionService';
import ClubAdvertisementCreateDTO from '../../models/dtos/ClubAdvertisementCreateDTO';
import PlayerPosition from '../../models/interfaces/PlayerPosition';
import '../../App.css';
import '../../styles/clubAdvertisement/NewClubAdvertisement.css';

const NewClubAdvertisement = () => {
    const navigate = useNavigate();
    const [userId, setUserId] = useState<string | null>(null);
    const [positions, setPositions] = useState<PlayerPosition[]>([]);
    const [clubAdvertisementDTO, setClubAdvertisementDTO] = useState<ClubAdvertisementCreateDTO>({
        playerPositionId: 0,
        clubName: '',
        league: '',
        region: '',
        salaryRangeDTO: {
            min: 0,
            max: 0,
        },
        clubMemberId: ''
    });

    useEffect(() => {
        const fetchUserData = async () => {
            try {
                const userId = await AccountService.getId();
                setUserId(userId);
            }
            catch (error) {
                console.error('Failed to fetch userId:', error);
                toast.error('Failed to load userId.');
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

    const handleCreate = async (e: React.FormEvent) => {
        if (!userId)
            return;

        const validationError = validateForm(clubAdvertisementDTO);
        if (validationError) {
            toast.error(validationError);
            return;
        }

        try {
            const createFormData = { ...clubAdvertisementDTO, clubMemberId: userId };
            await ClubAdvertisementService.createClubAdvertisement(createFormData);
            toast.success('Club advertisement created successfully!');
            navigate('/my-club-advertisements');
        }
        catch (error) {
            console.error('Failed to create club advertisement:', error);
            toast.error('Failed to create club advertisement.');
        }
    };

    const validateForm = (formData: ClubAdvertisementCreateDTO) => {
        const { playerPositionId, clubName, league, region, salaryRangeDTO } = formData;
        const { min, max } = salaryRangeDTO;

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

    return (
        <div className="NewClubAdvertisement">
            <h1><i className="bi bi-file-earmark-plus-fill"></i> New Club Advertisement</h1>
            <p></p>
            <div className="forms-container">
                <Container>
                    <Row className="justify-content-md-center">
                        <Col md="6">
                            <Form onSubmit={handleCreate}>
                                <Row>
                                    <Form.Group className="mb-3" controlId="formClubName">
                                        <Form.Label className="white-label">Club Name</Form.Label>
                                        <Form.Control
                                            type="text"
                                            placeholder="Club Name"
                                            value={clubAdvertisementDTO.clubName}
                                            onChange={(e) => setClubAdvertisementDTO({ ...clubAdvertisementDTO, clubName: e.target.value })}
                                            maxLength={30}
                                            required
                                        />
                                    </Form.Group>
                                </Row>
                                <Row>
                                    <Form.Group className="mb-3" controlId="formLeague">
                                        <Form.Label className="white-label">League</Form.Label>
                                        <Form.Control
                                            type="text"
                                            placeholder="League"
                                            value={clubAdvertisementDTO.league}
                                            onChange={(e) => setClubAdvertisementDTO({ ...clubAdvertisementDTO, league: e.target.value })}
                                            maxLength={30}
                                            required
                                        />
                                    </Form.Group>
                                </Row>
                                <Row>
                                    <Form.Group className="mb-3" controlId="formRegion">
                                        <Form.Label className="white-label">Region</Form.Label>
                                        <Form.Control
                                            type="text"
                                            placeholder="Region"
                                            value={clubAdvertisementDTO.region}
                                            onChange={(e) => setClubAdvertisementDTO({ ...clubAdvertisementDTO, region: e.target.value })}
                                            maxLength={30}
                                            required
                                        />
                                    </Form.Group>
                                </Row>
                                <Form.Group className="mb-3" controlId="formPosition">
                                    <Form.Label className="white-label">Position</Form.Label>
                                    <FormSelect
                                        value={clubAdvertisementDTO.playerPositionId}
                                        onChange={(e) => setClubAdvertisementDTO({
                                            ...clubAdvertisementDTO,
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
                                            <Form.Label className="white-label">Min Salary (zł.)/month</Form.Label>
                                            <Form.Control
                                                type="number"
                                                placeholder="Min"
                                                value={clubAdvertisementDTO.salaryRangeDTO.min}
                                                onChange={(e) => setClubAdvertisementDTO({
                                                    ...clubAdvertisementDTO,
                                                    salaryRangeDTO: {
                                                        ...clubAdvertisementDTO.salaryRangeDTO,
                                                        min: parseFloat(e.target.value)
                                                    }
                                                })}
                                                required
                                            />
                                        </Form.Group>
                                    </Col>
                                    <Col>
                                        <Form.Group className="mb-3" controlId="formMax">
                                            <Form.Label className="white-label">Max Salary (zł.)/month</Form.Label>
                                            <Form.Control
                                                type="number"
                                                placeholder="Max"
                                                value={clubAdvertisementDTO.salaryRangeDTO.max}
                                                onChange={(e) => setClubAdvertisementDTO({
                                                    ...clubAdvertisementDTO,
                                                    salaryRangeDTO: {
                                                        ...clubAdvertisementDTO.salaryRangeDTO,
                                                        max: parseFloat(e.target.value)
                                                    }
                                                })}
                                                required
                                            />
                                        </Form.Group>
                                    </Col>
                                </Row>
                                <Button variant="success" className="mb-3" onClick={handleCreate}>
                                    <i className="bi bi-file-earmark-plus-fill"></i>
                                    Create an advertisement
                                </Button>
                            </Form>
                        </Col>
                    </Row>
                </Container>
            </div>
        </div>
    );
}

export default NewClubAdvertisement;