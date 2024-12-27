import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { toast } from 'react-toastify';
import { Button } from 'react-bootstrap';
import { BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer } from 'recharts';
import PlayerOfferService from '../../services/api/PlayerOfferService';
import TimeService from '../../services/time/TimeService';
import '../../App.css';
import '../../styles/admin/AdminRaportsPlayerOffers.css';

const AdminRaportsPlayerOffers = () => {
    const navigate = useNavigate();
    const [playerOfferCount, setPlayerOfferCount] = useState<number>(0);
    const [playerOffersCreationData, setPlayerOffersCreationData] = useState<{ date: string, count: number }[]>([]);
    const [currentMonth, setCurrentMonth] = useState<Date>(new Date());

    useEffect(() => {
        const fetchPlayerOfferData = async () => {
            try {
                const _playerOffers = await PlayerOfferService.getPlayerOffers();

                if (_playerOffers.length > 0) {
                    const firstUserDate = new Date(Math.min(..._playerOffers.map(offer => new Date(offer.creationDate).getTime()))).toISOString().split('T')[0];
                    const todayDate = new Date().toISOString().split('T')[0];

                    const dateRange = TimeService.generateDateRange(firstUserDate, todayDate);

                    const creationCounts: { [key: string]: number } = {};
                    _playerOffers.forEach(offer => {
                        const creationDate = new Date(offer.creationDate).toISOString().split('T')[0];
                        creationCounts[creationDate] = (creationCounts[creationDate] || 0) + 1;
                    });

                    const formattedData = dateRange.map(date => ({
                        date,
                        count: creationCounts[date] || 0,
                    }));

                    setPlayerOffersCreationData(formattedData.sort((a, b) => new Date(a.date).getTime() - new Date(b.date).getTime()));
                    setPlayerOfferCount(_playerOffers.length);
                }
            }
            catch (error) {
                console.error('Failed to fetch player offer data:', error);
                toast.error('Failed to load player offer data.');
            }
        };

        fetchPlayerOfferData();
    }, []);

    const changeMonth = (direction: number) => {
        setCurrentMonth(prev => {
            const newDate = new Date(prev);
            newDate.setMonth(newDate.getMonth() + direction);
            return newDate;
        });
    };

    const playerOffersFilteredData = playerOffersCreationData.filter(item => item.date.startsWith(TimeService.formatDateToMonth(currentMonth)));

    const exportDataToCSV = async () => {
        await PlayerOfferService.exportPlayerOffersToCsv();
    };

    return (
        <div className="AdminRaportsPlayerOffers">
            <h1><i className="bi bi-briefcase"></i> Player Offers - Raports & Stats</h1>
            <p></p>
            <h3>Player Offers count: <strong>{playerOfferCount}</strong></h3>
            <p></p>
            <Button variant="primary" className="button-spacing" onClick={() => navigate('/admin/player-offers')}>
                <i className="bi bi-info-circle"></i> Show Player Offers
            </Button>
            <Button variant="success" onClick={exportDataToCSV}>
                <i className="bi bi-download"></i> Export to CSV
            </Button>
            <p></p>

            {/* Histogram */}
            <h5>Offer send dates for {TimeService.formatDateToMonth(currentMonth)}</h5>
            <div className="histogram-container">
                <ResponsiveContainer width="100%" height={350}>
                    <BarChart data={playerOffersFilteredData}>
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

export default AdminRaportsPlayerOffers;