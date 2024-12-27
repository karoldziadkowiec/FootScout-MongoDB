import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { toast } from 'react-toastify';
import { Button } from 'react-bootstrap';
import { BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer } from 'recharts';
import ClubOfferService from '../../services/api/ClubOfferService';
import TimeService from '../../services/time/TimeService';
import '../../App.css';
import '../../styles/admin/AdminRaportsClubOffers.css';

const AdminRaportsClubOffers = () => {
    const navigate = useNavigate();
    const [clubOfferCount, setClubOfferCount] = useState<number>(0);
    const [clubOffersCreationData, setClubOffersCreationData] = useState<{ date: string, count: number }[]>([]);
    const [currentMonth, setCurrentMonth] = useState<Date>(new Date());

    useEffect(() => {
        const fetchClubOfferData = async () => {
            try {
                const _clubOffers = await ClubOfferService.getClubOffers();

                if (_clubOffers.length > 0) {
                    const firstUserDate = new Date(Math.min(..._clubOffers.map(offer => new Date(offer.creationDate).getTime()))).toISOString().split('T')[0];
                    const todayDate = new Date().toISOString().split('T')[0];

                    const dateRange = TimeService.generateDateRange(firstUserDate, todayDate);

                    const creationCounts: { [key: string]: number } = {};
                    _clubOffers.forEach(offer => {
                        const creationDate = new Date(offer.creationDate).toISOString().split('T')[0];
                        creationCounts[creationDate] = (creationCounts[creationDate] || 0) + 1;
                    });

                    const formattedData = dateRange.map(date => ({
                        date,
                        count: creationCounts[date] || 0,
                    }));

                    setClubOffersCreationData(formattedData.sort((a, b) => new Date(a.date).getTime() - new Date(b.date).getTime()));
                    setClubOfferCount(_clubOffers.length);
                }
            }
            catch (error) {
                console.error('Failed to fetch club offer data:', error);
                toast.error('Failed to load club offer data.');
            }
        };

        fetchClubOfferData();
    }, []);

    const changeMonth = (direction: number) => {
        setCurrentMonth(prev => {
            const newDate = new Date(prev);
            newDate.setMonth(newDate.getMonth() + direction);
            return newDate;
        });
    };

    const clubOffersFilteredData = clubOffersCreationData.filter(item => item.date.startsWith(TimeService.formatDateToMonth(currentMonth)));

    const exportDataToCSV = async () => {
        await ClubOfferService.exportClubOffersToCsv();
    };

    return (
        <div className="AdminRaportsClubOffers">
            <h1><i className="bi bi-briefcase-fill"></i> Club Offers - Raports & Stats</h1>
            <p></p>
            <h3>Club Offers count: <strong>{clubOfferCount}</strong></h3>
            <p></p>
            <Button variant="primary" className="button-spacing" onClick={() => navigate('/admin/club-offers')}>
                <i className="bi bi-info-circle"></i> Show Club Offers
            </Button>
            <Button variant="success" onClick={exportDataToCSV}>
                <i className="bi bi-download"></i> Export to CSV
            </Button>
            <p></p>

            {/* Histogram */}
            <h5>Offer send dates for {TimeService.formatDateToMonth(currentMonth)}</h5>
            <div className="histogram-container">
                <ResponsiveContainer width="100%" height={350}>
                    <BarChart data={clubOffersFilteredData}>
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

export default AdminRaportsClubOffers;