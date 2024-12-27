import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { toast } from 'react-toastify';
import { Button } from 'react-bootstrap';
import { BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer } from 'recharts';
import PlayerAdvertisementService from '../../services/api/PlayerAdvertisementService';
import TimeService from '../../services/time/TimeService';
import '../../App.css';
import '../../styles/admin/AdminRaportsPlayerAdvertisements.css';

const AdminRaportsPlayerAdvertisements = () => {
    const navigate = useNavigate();
    const [playerAdvertisementCount, setPlayerAdvertisementCount] = useState<number>(0);
    const [playerAdvertisementCreationData, setPlayerAdvertisementCreationData] = useState<{ date: string, count: number }[]>([]);
    const [currentMonth, setCurrentMonth] = useState<Date>(new Date());

    useEffect(() => {
        const fetchPlayerAdvertisementData = async () => {
            try {
                const _playerAdvertisements = await PlayerAdvertisementService.getAllPlayerAdvertisements();

                if (_playerAdvertisements.length > 0) {
                    const firstUserDate = new Date(Math.min(..._playerAdvertisements.map(advertisement => new Date(advertisement.creationDate).getTime()))).toISOString().split('T')[0];
                    const todayDate = new Date().toISOString().split('T')[0];

                    const dateRange = TimeService.generateDateRange(firstUserDate, todayDate);

                    const creationCounts: { [key: string]: number } = {};
                    _playerAdvertisements.forEach(advertisement => {
                        const creationDate = new Date(advertisement.creationDate).toISOString().split('T')[0];
                        creationCounts[creationDate] = (creationCounts[creationDate] || 0) + 1;
                    });

                    const formattedData = dateRange.map(date => ({
                        date,
                        count: creationCounts[date] || 0,
                    }));

                    setPlayerAdvertisementCreationData(formattedData.sort((a, b) => new Date(a.date).getTime() - new Date(b.date).getTime()));
                    setPlayerAdvertisementCount(_playerAdvertisements.length);
                }
            }
            catch (error) {
                console.error('Failed to fetch player advertisement data:', error);
                toast.error('Failed to load player advertisement data.');
            }
        };

        fetchPlayerAdvertisementData();
    }, []);

    const changeMonth = (direction: number) => {
        setCurrentMonth(prev => {
            const newDate = new Date(prev);
            newDate.setMonth(newDate.getMonth() + direction);
            return newDate;
        });
    };

    const playerAdvertisementFilteredData = playerAdvertisementCreationData.filter(item => item.date.startsWith(TimeService.formatDateToMonth(currentMonth)));

    const exportDataToCSV = async () => {
        await PlayerAdvertisementService.exportPlayerAdvertisementsToCsv();
    };

    return (
        <div className="AdminRaportsPlayerAdvertisements">
            <h1><i className="bi bi-person-bounding-box"></i> Player Advertisements - Raports & Stats</h1>
            <p></p>
            <h3>Player Advertisements count: <strong>{playerAdvertisementCount}</strong></h3>
            <p></p>
            <Button variant="primary" className="button-spacing" onClick={() => navigate('/admin/player-advertisements')}>
                <i className="bi bi-info-circle"></i> Show Player Advertisements
            </Button>
            <Button variant="success" onClick={exportDataToCSV}>
                <i className="bi bi-download"></i> Export to CSV
            </Button>
            <p></p>

            {/* Histogram */}
            <h5>Creation dates for {TimeService.formatDateToMonth(currentMonth)}</h5>
            <div className="histogram-container">
                <ResponsiveContainer width="100%" height={350}>
                    <BarChart data={playerAdvertisementFilteredData}>
                        <CartesianGrid strokeDasharray="3 3" />
                        <XAxis dataKey="date" />
                        <YAxis />
                        <Tooltip />
                        <Bar dataKey="count" fill="#8884d8" />
                    </BarChart>
                </ResponsiveContainer>
                <Button variant="dark" className="button-spacing" onClick={() => changeMonth(-1)}>
                    <i className="bi bi-arrow-left"></i>
                </Button>
                <Button variant="dark" onClick={() => changeMonth(1)}>
                    <i className="bi bi-arrow-right"></i>
                </Button>
            </div>
        </div>
    );
};

export default AdminRaportsPlayerAdvertisements;