import { connect } from 'react-redux';
import { componentDidMount } from 'react-lifecycle-decorators';
import { Switch, Route, Link } from 'react-router-dom';

import AddSvn from './addSVN';

let AddSources = ({ match, sourceControlTypes }) => (
    <Switch>
        <Route exact path={ match.url } render={ () =>
            <div className="container">
                <h1>
                    Add source
                </h1>
                <hr/>
                <div className="row sourceSelector" >
                    <div className="col-md-6">
                        <div className="list-group">
                            <Link to={match.url + '/SVN' } className="list-group-item">
                                <h3>
                                    <img style={{marginRight: '20px', height: '50px'}} src="/Resources/Layout/Images/Icons/SVN-Icon.jpg" />
                                    SVN
                                </h3>
                            </Link>
                        </div>
                        <div className="list-group">
                            <Link to={match.url + '/FileSystem' } className="list-group-item">
                                <h3>
                                    <img style={{marginRight: '20px', height: '50px'}} src="/Resources/Layout/Images/Icons/fileSystem.png" />
                                    File system
                                </h3>
                            </Link>
                        </div>
                    </div>
                </div>
            </div>
        } />
        <Route path={ match.url + "/SVN" } component={AddSvn} />
        <Route path={ match.url + "/FileSystem" } />
    </Switch>
);

AddSources = componentDidMount(
    (props) => {
    }
)(AddSources);

const mapStateToProps = (state) => {
    return state;
};

export default connect(mapStateToProps)(AddSources);
