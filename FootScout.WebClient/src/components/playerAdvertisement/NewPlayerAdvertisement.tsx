import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { Form, Button, Col, Row, Container, FormSelect } from 'react-bootstrap';
import { toast } from 'react-toastify';
import AccountService from '../../services/api/AccountService';
import PlayerAdvertisementService from '../../services/api/PlayerAdvertisementService';
import PlayerPositionService from '../../services/api/PlayerPositionService';
import PlayerFootService from '../../services/api/PlayerFootService';
import PlayerAdvertisementCreateDTO from '../../models/dtos/PlayerAdvertisementCreateDTO';
import PlayerPosition from '../../models/interfaces/PlayerPosition';
import PlayerFoot from '../../models/interfaces/PlayerFoot';
import '../../App.css';
import '../../styles/playerAdvertisement/NewPlayerAdvertisement.css';

const NewPlayerAdvertisement = () => {
    const navigate = useNavigate();
    const [userId, setUserId] = useState<string | null>(null);
    const [positions, setPositions] = useState<PlayerPosition[]>([]);
    const [feet, setFeet] = useState<PlayerFoot[]>([]);
    const [playerAdvertisementDTO, setPlayerAdvertisementDTO] = useState<PlayerAdvertisementCreateDTO>({
        playerPositionId: 0,
        league: '',
        region: '',
        age: 0,
        height: 0,
        playerFootId: 0,
        salaryRangeDTO: {
            min: 0,
            max: 0,
        },
        playerId: ''
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

        fetchUserData();
        fetchPositions();
        fetchFeet();
    }, []);

    const handleCreate = async (e: React.FormEvent) => {
        if (!userId)
            return;

        const validationError = validateForm(playerAdvertisementDTO);
        if (validationError) {
            toast.error(validationError);
            return;
        }

        try {
            const createFormData = { ...playerAdvertisementDTO, playerId: userId };
            await PlayerAdvertisementService.createPlayerAdvertisement(createFormData);
            toast.success('Player advertisement created successfully!');
            navigate('/my-player-advertisements');
        }
        catch (error) {
            console.error('Failed to create player advertisement:', error);
            toast.error('Failed to create player advertisement.');
        }
    };

    const validateForm = (formData: PlayerAdvertisementCreateDTO) => {
        const { playerPositionId, league, region, age, height, playerFootId, salaryRangeDTO } = formData;
        const { min, max } = salaryRangeDTO;

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

    return (
        <div className="NewPlayerAdvertisement">
            <h1><i className="bi bi-file-earmark-plus"></i> New Player Advertisement</h1>
            <p></p>
            <div className="forms-container">
                <Container>
                    <Row className="justify-content-md-center">
                        <Col md="6">
                            <Form onSubmit={handleCreate}>
                                <Form.Group className="mb-3" controlId="formPosition">
                                    <Form.Label className="white-label">Position</Form.Label>
                                    <FormSelect
                                        value={playerAdvertisementDTO.playerPositionId}
                                        onChange={(e) => setPlayerAdvertisementDTO({
                                            ...playerAdvertisementDTO,
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
                                            <Form.Label className="white-label">Preferred League</Form.Label>
                                            <Form.Control
                                                type="text"
                                                placeholder="League"
                                                value={playerAdvertisementDTO.league}
                                                onChange={(e) => setPlayerAdvertisementDTO({ ...playerAdvertisementDTO, league: e.target.value })}
                                                maxLength={30}
                                                required
                                            />
                                        </Form.Group>
                                    </Col>
                                    <Col>
                                        <Form.Group className="mb-3" controlId="formRegion">
                                            <Form.Label className="white-label">Region</Form.Label>
                                            <Form.Control
                                                type="text"
                                                placeholder="Region"
                                                value={playerAdvertisementDTO.region}
                                                onChange={(e) => setPlayerAdvertisementDTO({ ...playerAdvertisementDTO, region: e.target.value })}
                                                maxLength={30}
                                                required
                                            />
                                        </Form.Group>
                                    </Col>
                                </Row>
                                <Row>
                                    <Col>
                                        <Form.Group className="mb-3" controlId="formAge">
                                            <Form.Label className="white-label">Age</Form.Label>
                                            <Form.Control
                                                type="number"
                                                placeholder="Age"
                                                value={playerAdvertisementDTO.age}
                                                onChange={(e) => setPlayerAdvertisementDTO({ ...playerAdvertisementDTO, age: parseInt(e.target.value) })}
                                                required
                                            />
                                        </Form.Group>
                                    </Col>
                                    <Col>
                                        <Form.Group className="mb-3" controlId="formHeight">
                                            <Form.Label className="white-label">Height (cm)</Form.Label>
                                            <Form.Control
                                                type="number"
                                                placeholder="Height"
                                                value={playerAdvertisementDTO.height}
                                                onChange={(e) => setPlayerAdvertisementDTO({ ...playerAdvertisementDTO, height: parseInt(e.target.value) })}
                                                required
                                            />
                                        </Form.Group>
                                    </Col>
                                </Row>
                                <Form.Group className="mb-3" controlId="formFoot">
                                    <Form.Label className="white-label">Foot</Form.Label>
                                    <FormSelect
                                        value={playerAdvertisementDTO.playerFootId}
                                        onChange={(e) => setPlayerAdvertisementDTO({
                                            ...playerAdvertisementDTO,
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
                                            <Form.Label className="white-label">Min Salary (zł.)/month</Form.Label>
                                            <Form.Control
                                                type="number"
                                                placeholder="Min"
                                                value={playerAdvertisementDTO.salaryRangeDTO.min}
                                                onChange={(e) => setPlayerAdvertisementDTO({
                                                    ...playerAdvertisementDTO,
                                                    salaryRangeDTO: {
                                                        ...playerAdvertisementDTO.salaryRangeDTO,
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
                                                value={playerAdvertisementDTO.salaryRangeDTO.max}
                                                onChange={(e) => setPlayerAdvertisementDTO({
                                                    ...playerAdvertisementDTO,
                                                    salaryRangeDTO: {
                                                        ...playerAdvertisementDTO.salaryRangeDTO,
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

export default NewPlayerAdvertisement;