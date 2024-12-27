import React, { useEffect, useState } from 'react';
import { Table, Form, Button, Row, Col, Modal } from 'react-bootstrap';
import { toast } from 'react-toastify';
import PlayerPositionService from '../../services/api/PlayerPositionService';
import PlayerPosition from '../../models/interfaces/PlayerPosition';
import '../../App.css';
import '../../styles/admin/AdminPlayerPositions.css';

const AdminPlayerPositions = () => {
    const [positions, setPositions] = useState<PlayerPosition[]>([]);
    const [positionCount, setPositionCount] = useState<number>(0);
    const [showCreateModal, setShowCreateModal] = useState<boolean>(false);
    const [createPositionForm, setCreatePositionForm] = useState<PlayerPosition>({
        id: 0,
        positionName: '',
    }
    );

    useEffect(() => {
        const fetchPositionsData = async () => {
            try {
                const _positions = await PlayerPositionService.getPlayerPositions();
                setPositions(_positions);

                const _positionCount = await PlayerPositionService.getPlayerPositionCount();
                setPositionCount(_positionCount);
            }
            catch (error) {
                console.error('Failed to fetch positions:', error);
                toast.error('Failed to load positions.');
            }
        };

        fetchPositionsData();
    }, []);

    const handleCreatePosition = async () => {
        if (!createPositionForm.positionName) {
            toast.error('Position Name field is required!');
            return;
        }

        try {
            let isExists = await PlayerPositionService.checkPlayerPositionExists(createPositionForm.positionName);
            if (isExists === true) {
                toast.error('Position Name already exists.');
                return;
            }
            else {
                await PlayerPositionService.createPlayerPosition(createPositionForm);
                setShowCreateModal(false);
                toast.success('Position created successfully!');
                // Refresh the user data
                const _positions = await PlayerPositionService.getPlayerPositions();
                setPositions(_positions);
                const _positionCount = await PlayerPositionService.getPlayerPositionCount();
                setPositionCount(_positionCount);
            }
        }
        catch (error) {
            console.error('Failed to create new position:', error);
            toast.error('Failed to create new position.');
        }
    };

    return (
        <div className="AdminPlayerPositions">
            <h1><i className="bi bi-person-standing"></i> Player Positions</h1>
            <p></p>
            <h3>Count: <strong>{positionCount}</strong></h3>
            <p></p>
            <Button variant="primary" className="form-button" onClick={() => setShowCreateModal(true)}>
                <i className="bi bi-file-earmark-plus"></i>
                Create New Position
            </Button>
            <p></p>
            <div className="table-responsive">
                <Table striped bordered hover variant="secondary">
                    <thead className="table-dark">
                        <tr>
                            <th>Position</th>
                        </tr>
                    </thead>
                    <tbody>
                        {positions.length > 0 ? (
                            positions.map((position, index) => (
                                <tr key={index}>
                                    <td>{position.positionName}</td>
                                </tr>
                            ))
                        ) : (
                            <tr>
                                <td colSpan={9} className="text-center">No positions available</td>
                            </tr>
                        )}
                    </tbody>
                </Table>
            </div>

            {/* Create Position Modal */}
            <Modal show={showCreateModal} onHide={() => setShowCreateModal(false)}>
                <Modal.Header closeButton>
                    <Modal.Title>Create New Position</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <Form>
                        <Form.Group as={Row} controlId="formPositionName">
                            <Form.Label column sm="3">Position Name</Form.Label>
                            <Col sm="9">
                                <Form.Control
                                    type="text"
                                    placeholder="Position Name"
                                    value={createPositionForm.positionName}
                                    onChange={(e) => setCreatePositionForm({ ...createPositionForm, positionName: e.target.value })}
                                    maxLength={30}
                                    required
                                />
                            </Col>
                        </Form.Group>
                    </Form>
                </Modal.Body>
                <Modal.Footer>
                    <Button variant="secondary" onClick={() => setShowCreateModal(false)}>Close</Button>
                    <Button variant="primary" onClick={handleCreatePosition}>Create</Button>
                </Modal.Footer>
            </Modal>
        </div>
    );
}

export default AdminPlayerPositions;